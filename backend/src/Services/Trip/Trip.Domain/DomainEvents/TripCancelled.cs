using Trip.Domain.Enumerators;

namespace Trip.Domain.DomainEvents;

public sealed record TripCancelled(
    long TripId,
    CancelledBy CancelledBy,
    string? Reason,
    DateTime CancelledAt) : IDomainEvent;
