using Moq;
using Trip.Application.Abstractions;
using Trip.Application.Dtos;
using Trip.Application.Exceptions;
using Trip.Application.Services;
using Trip.Domain.Entities;
using Trip.Domain.Enums;
using Trip.Domain.Events;
using Trip.Domain.Exceptions;
using Trip.Domain.ValueObjects;

namespace Trip.Application.Tests;

public class TripServiceTests
{
    private readonly Mock<ITripReader> _reader = new();
    private readonly Mock<ITripWriter> _writer = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMatchingClient> _matchingClient = new();
    private readonly Mock<IPaymentGateway> _paymentGateway = new();
    private readonly Mock<IEventBus> _eventBus = new();
    private readonly List<object> _publishedEvents = new();

    public TripServiceTests()
    {
        _writer
            .Setup(w => w.AddAsync(It.IsAny<TripEntity>(), It.IsAny<CancellationToken>()))
            .Callback<TripEntity, CancellationToken>((trip, _) => SetId(trip, 42))
            .Returns(Task.CompletedTask);
        _writer
            .Setup(w => w.UpdateAsync(It.IsAny<TripEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _paymentGateway
            .Setup(p => p.RequestPaymentAsync(It.IsAny<PaymentRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _eventBus
            .Setup(b => b.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((domainEvent, _) => _publishedEvents.Add(domainEvent))
            .Returns(Task.CompletedTask);
    }

    private TripService CreateService() =>
        new(
            _reader.Object,
            _writer.Object,
            _unitOfWork.Object,
            _matchingClient.Object,
            _paymentGateway.Object,
            _eventBus.Object);

    [Fact]
    public async Task RequestTrip_CreatesQuotedTrip_AndPublishesTripRequested()
    {
        _matchingClient
            .Setup(m => m.MatchAsync(It.IsAny<MatchingRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MatchingResult(90000m, 501, 240));

        var response = await CreateService().RequestTripAsync(new RequestTripRequest
        {
            RiderId = 7,
            PickupAddress = "A",
            PickupLatitude = 10.1m,
            PickupLongitude = 106.1m,
            DropoffAddress = "B",
            DropoffLatitude = 10.2m,
            DropoffLongitude = 106.2m
        });

        Assert.Equal(42, response.Id);
        Assert.Equal("Quoted", response.Status);
        Assert.Equal(90000m, response.Cost);
        Assert.Equal(501, response.DriverId);
        Assert.Contains(_publishedEvents, e => e is TripRequested { TripId: 42 });
    }

    [Fact]
    public async Task GetAsync_UnknownTrip_ThrowsNotFound()
    {
        _reader
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TripEntity?)null);

        await Assert.ThrowsAsync<TripNotFoundException>(() => CreateService().GetAsync(99));
    }

    [Fact]
    public async Task Dropoff_PublishesRideCompletedWithStableIdempotencyKey()
    {
        var trip = BuildPickedUpTrip();
        SetId(trip, 7);
        _reader
            .Setup(r => r.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        var response = await CreateService().DropoffAsync(7);

        Assert.Equal("PaymentPending", response.Status);
        var completed = Assert.Single(_publishedEvents.OfType<RideCompleted>());
        Assert.Equal(7, completed.TripId);
        Assert.Equal(85000m, completed.Amount);
        Assert.Equal("trip-7-payment", completed.IdempotencyKey);
    }

    [Fact]
    public async Task PaymentSettled_MarksTripAsPaid()
    {
        var trip = BuildPaymentPendingTrip();
        SetId(trip, 8);
        _reader
            .Setup(r => r.GetByIdAsync(8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        var response = await CreateService().OnPaymentSettledAsync(8);

        Assert.Equal("Paid", response.Status);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PaymentFailed_RetriesWithStableIdempotencyKey()
    {
        var trip = BuildPaymentPendingTrip();
        SetId(trip, 12);
        _reader
            .Setup(r => r.GetByIdAsync(12, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        var response = await CreateService().OnPaymentFailedAsync(12);

        Assert.Equal("PaymentPending", response.Status);
        _paymentGateway.Verify(
            p => p.RequestPaymentAsync(
                It.Is<PaymentRequest>(r =>
                    r.TripId == 12 &&
                    r.Amount == 85000m &&
                    r.IdempotencyKey == "trip-12-payment"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ConfirmQuote_WhenNotQuoted_ThrowsDomainException()
    {
        var trip = BuildPickedUpTrip();
        SetId(trip, 9);
        _reader
            .Setup(r => r.GetByIdAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        await Assert.ThrowsAsync<DomainException>(() => CreateService().ConfirmQuoteAsync(9));
    }

    [Fact]
    public async Task DriverReject_AboveThreshold_ReturnsFailed()
    {
        var trip = BuildAwaitingDriverTrip();
        SetId(trip, 10);
        _reader
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        TripResponse? response = null;
        var service = CreateService();

        for (var i = 0; i <= TripEntity.MaxDriverRejectCount; i++)
        {
            response = await service.DriverRejectAsync(10, 501);
        }

        Assert.NotNull(response);
        Assert.Equal("Failed", response!.Status);
        Assert.Equal("System", response.CancelledBy);
    }

    private static TripEntity BuildAwaitingDriverTrip()
    {
        var trip = BuildQuotedTrip();
        trip.ConfirmQuote();
        return trip;
    }

    private static TripEntity BuildPickedUpTrip()
    {
        var trip = BuildAwaitingDriverTrip();
        trip.DriverAccept(501);
        trip.Arrive();
        trip.Pickup();
        return trip;
    }

    private static TripEntity BuildPaymentPendingTrip()
    {
        var trip = BuildPickedUpTrip();
        trip.Dropoff();
        return trip;
    }

    private static TripEntity BuildQuotedTrip()
    {
        var trip = new TripEntity(
            7,
            "A",
            new GeoPoint(10.1m, 106.1m),
            "B",
            new GeoPoint(10.2m, 106.2m));
        trip.Quote(new Money(85000m), 501);
        return trip;
    }

    private static void SetId(TripEntity trip, long id) =>
        typeof(TripEntity).GetProperty(nameof(TripEntity.Id))!.SetValue(trip, id);
}
