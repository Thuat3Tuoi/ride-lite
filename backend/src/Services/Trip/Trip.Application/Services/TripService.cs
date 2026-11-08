using Trip.Application.Abstractions;
using Trip.Application.Dtos;
using Trip.Application.Exceptions;
using Trip.Domain.Entities;
using Trip.Domain.Events;
using Trip.Domain.ValueObjects;

namespace Trip.Application.Services;

public sealed class TripService
{
    private readonly ITripReader _reader;
    private readonly ITripWriter _writer;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMatchingClient _matchingClient;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IEventBus _eventBus;

    public TripService(
        ITripReader reader,
        ITripWriter writer,
        IUnitOfWork unitOfWork,
        IMatchingClient matchingClient,
        IPaymentGateway paymentGateway,
        IEventBus eventBus)
    {
        _reader = reader;
        _writer = writer;
        _unitOfWork = unitOfWork;
        _matchingClient = matchingClient;
        _paymentGateway = paymentGateway;
        _eventBus = eventBus;
    }

    public async Task<TripResponse> RequestTripAsync(
        RequestTripRequest request,
        CancellationToken cancellationToken = default)
    {
        var pickup = new GeoPoint(request.PickupLatitude, request.PickupLongitude);
        var dropoff = new GeoPoint(request.DropoffLatitude, request.DropoffLongitude);

        var trip = new TripEntity(
            request.RiderId,
            request.PickupAddress,
            pickup,
            request.DropoffAddress,
            dropoff);

        await _writer.AddAsync(trip, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var match = await _matchingClient.MatchAsync(
            new MatchingRequest(trip.Id, pickup, dropoff),
            cancellationToken);

        trip.Quote(new Money(match.Fare), match.DriverId);

        await _writer.UpdateAsync(trip, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventBus.PublishAsync(
            new TripRequested(trip.Id, trip.RiderId, pickup, dropoff, trip.CreatedAt),
            cancellationToken);
        await PublishDomainEventsAsync(trip, cancellationToken);

        return TripResponse.From(trip);
    }

    public async Task<TripResponse> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        var trip = await GetTripAsync(id, cancellationToken);
        return TripResponse.From(trip);
    }

    public async Task<TripResponse> ConfirmQuoteAsync(long id, CancellationToken cancellationToken = default)
    {
        var trip = await GetTripAsync(id, cancellationToken);
        trip.ConfirmQuote();
        return await PersistAsync(trip, cancellationToken);
    }

    public async Task<TripResponse> RejectQuoteAsync(long id, CancellationToken cancellationToken = default)
    {
        var trip = await GetTripAsync(id, cancellationToken);
        trip.RejectQuote();
        return await PersistAsync(trip, cancellationToken);
    }

    public async Task<TripResponse> DriverAcceptAsync(
        long id,
        long driverId,
        CancellationToken cancellationToken = default)
    {
        var trip = await GetTripAsync(id, cancellationToken);
        trip.DriverAccept(driverId);
        return await PersistAsync(trip, cancellationToken);
    }

    public async Task<TripResponse> DriverRejectAsync(
        long id,
        long driverId,
        CancellationToken cancellationToken = default)
    {
        var trip = await GetTripAsync(id, cancellationToken);
        trip.DriverReject(driverId);
        return await PersistAsync(trip, cancellationToken);
    }

    public async Task<TripResponse> ArriveAsync(long id, CancellationToken cancellationToken = default)
    {
        var trip = await GetTripAsync(id, cancellationToken);
        trip.Arrive();
        return await PersistAsync(trip, cancellationToken);
    }

    public async Task<TripResponse> PickupAsync(long id, CancellationToken cancellationToken = default)
    {
        var trip = await GetTripAsync(id, cancellationToken);
        trip.Pickup();
        return await PersistAsync(trip, cancellationToken);
    }

    public async Task<TripResponse> DropoffAsync(long id, CancellationToken cancellationToken = default)
    {
        var trip = await GetTripAsync(id, cancellationToken);
        trip.Dropoff();
        return await PersistAsync(trip, cancellationToken);
    }

    public async Task<TripResponse> OnPaymentSettledAsync(long id, CancellationToken cancellationToken = default)
    {
        var trip = await GetTripAsync(id, cancellationToken);
        trip.PaymentSettled();
        return await PersistAsync(trip, cancellationToken);
    }

    public async Task<TripResponse> OnPaymentFailedAsync(long id, CancellationToken cancellationToken = default)
    {
        var trip = await GetTripAsync(id, cancellationToken);
        trip.PaymentFailed();
        var response = await PersistAsync(trip, cancellationToken);

        if (trip.Cost is { } amount)
        {
            await _paymentGateway.RequestPaymentAsync(
                new PaymentRequest(trip.Id, trip.RiderId, amount, $"trip-{trip.Id}-payment"),
                cancellationToken);
        }

        return response;
    }

    private async Task<TripEntity> GetTripAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new TripValidationException("Trip id must be positive.");
        }

        return await _reader.GetByIdAsync(id, cancellationToken)
            ?? throw new TripNotFoundException(id);
    }

    private async Task<TripResponse> PersistAsync(TripEntity trip, CancellationToken cancellationToken)
    {
        await _writer.UpdateAsync(trip, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await PublishDomainEventsAsync(trip, cancellationToken);
        return TripResponse.From(trip);
    }

    private async Task PublishDomainEventsAsync(TripEntity trip, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in trip.DequeueDomainEvents())
        {
            await _eventBus.PublishAsync(domainEvent, cancellationToken);
        }
    }
}
