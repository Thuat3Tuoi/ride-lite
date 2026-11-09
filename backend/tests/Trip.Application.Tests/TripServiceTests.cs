using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Abstractions.Services;
using Trip.Application.Abstractions.Validation;
using Trip.Application.Contracts.Responses;
using Trip.Application.Exceptions;
using Trip.Application.Trips.Commands.Dropoff;
using Trip.Application.Trips.Commands.RequestTrip;
using Trip.Application.Trips.Queries.GetTripById;
using Trip.Domain.DomainEvents;
using Trip.Domain.Entities;
using Trip.Domain.Repositories;
using Trip.Domain.ValueObjects;

namespace Trip.Application.Tests;

public sealed class TripServiceTests
{
    [Fact]
    public async Task RequestTrip_CreatesQuotedTripAndPublishesRequestedEvent()
    {
        var reader = new Mock<ITripReader>();
        var writer = new Mock<ITripWriter>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var matchingClient = new Mock<IMatchingClient>();
        var eventBus = new Mock<IEventBus>();

        writer
            .Setup(repository => repository.AddAsync(It.IsAny<TripEntity>(), It.IsAny<CancellationToken>()))
            .Callback<TripEntity, CancellationToken>((trip, _) => SetId(trip, 42))
            .Returns(Task.CompletedTask);
        writer
            .Setup(repository => repository.UpdateAsync(It.IsAny<TripEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        unitOfWork
            .Setup(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        matchingClient
            .Setup(client => client.MatchAsync(It.IsAny<MatchingRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MatchingResult(90000m, 501, 240));

        var handler = new RequestTripHandler(
            reader.Object,
            writer.Object,
            unitOfWork.Object,
            eventBus.Object,
            matchingClient.Object,
            new RequestTripValidator(),
            NullLogger<RequestTripHandler>.Instance);

        var response = await handler.HandleAsync(new RequestTripCommand(
            7,
            "A",
            10.1m,
            106.1m,
            "B",
            10.2m,
            106.2m), TestContext.Current.CancellationToken);

        Assert.Equal(42, response.Id);
        Assert.Equal("Quoted", response.Status);
        Assert.Equal(90000m, response.Cost);
        Assert.Equal(501, response.DriverId);
        eventBus.Verify(
            bus => bus.PublishAsync(
                It.Is<TripRequested>(domainEvent => domainEvent.TripId == 42),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dropoff_PublishesRideCompletedWithStableIdempotencyKey()
    {
        var trip = BuildPickedUpTrip();
        SetId(trip, 7);
        var reader = new Mock<ITripReader>();
        reader
            .Setup(repository => repository.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(trip);

        var eventBus = new Mock<IEventBus>();
        var handler = new DropoffHandler(
            reader.Object,
            new Mock<ITripWriter>().Object,
            CreateUnitOfWork().Object,
            eventBus.Object,
            new DropoffValidator(),
            NullLogger<DropoffHandler>.Instance);

        var response = await handler.HandleAsync(
            new DropoffCommand(7),
            TestContext.Current.CancellationToken);

        Assert.Equal("PaymentPending", response.Status);
        eventBus.Verify(
            bus => bus.PublishAsync(
                It.Is<RideCompleted>(domainEvent =>
                    domainEvent.TripId == 7 &&
                    domainEvent.Amount == 85000m &&
                    domainEvent.IdempotencyKey == "trip-7-payment"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTripById_WhenTripDoesNotExist_ThrowsNotFound()
    {
        var reader = new Mock<ITripReader>();
        reader
            .Setup(repository => repository.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TripEntity?)null);

        var handler = new GetTripByIdHandler(reader.Object, new GetTripByIdValidator());

        await Assert.ThrowsAsync<TripNotFoundException>(() =>
            handler.HandleAsync(new GetTripByIdQuery(99), TestContext.Current.CancellationToken));
    }

    private static Mock<IUnitOfWork> CreateUnitOfWork()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork
            .Setup(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        return unitOfWork;
    }

    private static TripEntity BuildPickedUpTrip()
    {
        var trip = new TripEntity(
            7,
            "A",
            new GeoPoint(10.1m, 106.1m),
            "B",
            new GeoPoint(10.2m, 106.2m));
        trip.Quote(new Money(85000m), 501);
        trip.ConfirmQuote();
        trip.DriverAccept(501);
        trip.Arrive();
        trip.Pickup();
        return trip;
    }

    private static void SetId(TripEntity trip, long id) =>
        typeof(TripEntity).BaseType!.GetProperty(nameof(TripEntity.Id))!.SetValue(trip, id);
}
