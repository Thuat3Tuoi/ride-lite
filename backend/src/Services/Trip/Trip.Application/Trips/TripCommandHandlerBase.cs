using Trip.Application.Abstractions.Data;
using Trip.Application.Abstractions.Messaging;
using Trip.Application.Behaviors;
using Trip.Application.Contracts.Responses;
using Trip.Application.Exceptions;
using Trip.Domain.Entities;
using Trip.Domain.Repositories;

namespace Trip.Application.Trips;

public abstract class TripCommandHandlerBase
{
    private readonly ITripReader _reader;
    private readonly ITripWriter _writer;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;

    protected TripCommandHandlerBase(
        ITripReader reader,
        ITripWriter writer,
        IUnitOfWork unitOfWork,
        IEventBus eventBus)
    {
        _reader = reader;
        _writer = writer;
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
    }

    protected ITripWriter Writer => _writer;

    protected IUnitOfWork UnitOfWork => _unitOfWork;

    protected IEventBus EventBus => _eventBus;

    protected async Task<TripEntity> LoadTripAsync(long tripId, CancellationToken cancellationToken)
    {
        if (tripId <= 0)
        {
            throw new TripValidationException("Trip id must be positive.");
        }

        return await _reader.GetByIdAsync(tripId, cancellationToken)
            ?? throw new TripNotFoundException(tripId);
    }

    protected async Task<TripResponse> PersistAsync(TripEntity trip, CancellationToken cancellationToken)
    {
        await _writer.UpdateAsync(trip, cancellationToken);
        await TransactionBehavior.SaveChangesAsync(_unitOfWork, cancellationToken);
        await PublishDomainEventsAsync(trip, cancellationToken);
        return TripResponse.From(trip);
    }

    protected async Task PublishDomainEventsAsync(TripEntity trip, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in trip.DequeueDomainEvents())
        {
            await _eventBus.PublishAsync(domainEvent, cancellationToken);
        }
    }
}
