using Microsoft.Extensions.Logging;
using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Abstractions.Services;
using Trip.Application.Abstractions.Validation;
using Trip.Application.Behaviors;
using Trip.Application.Contracts.Responses;
using Trip.Domain.DomainEvents;
using Trip.Domain.Entities;
using Trip.Domain.Repositories;
using Trip.Domain.ValueObjects;

namespace Trip.Application.Trips.Commands.RequestTrip;

public sealed class RequestTripHandler : TripCommandHandlerBase, ICommandHandler<RequestTripCommand, TripResponse>
{
    private readonly IMatchingClient _matchingClient;
    private readonly IValidator<RequestTripCommand> _validator;
    private readonly ILogger<RequestTripHandler> _logger;

    public RequestTripHandler(
        ITripReader reader,
        ITripWriter writer,
        IUnitOfWork unitOfWork,
        IEventBus eventBus,
        IMatchingClient matchingClient,
        IValidator<RequestTripCommand> validator,
        ILogger<RequestTripHandler> logger)
        : base(reader, writer, unitOfWork, eventBus)
    {
        _matchingClient = matchingClient;
        _validator = validator;
        _logger = logger;
    }

    public async Task<TripResponse> HandleAsync(
        RequestTripCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidationBehavior.Validate(_validator, command);

        var pickup = new GeoPoint(command.PickupLatitude, command.PickupLongitude);
        var dropoff = new GeoPoint(command.DropoffLatitude, command.DropoffLongitude);

        var trip = new TripEntity(
            command.RiderId,
            command.PickupAddress,
            pickup,
            command.DropoffAddress,
            dropoff);

        await Writer.AddAsync(trip, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var match = await _matchingClient.MatchAsync(
            new MatchingRequest(trip.Id, pickup, dropoff),
            cancellationToken);

        trip.Quote(new Money(match.Fare), match.DriverId);

        await Writer.UpdateAsync(trip, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        await EventBus.PublishAsync(
            new TripRequested(trip.Id, trip.RiderId, pickup, dropoff, trip.CreatedAt),
            cancellationToken);
        await PublishDomainEventsAsync(trip, cancellationToken);

        LoggingBehavior.LogHandled(_logger, nameof(RequestTripCommand), trip.Id);

        return TripResponse.From(trip);
    }
}
