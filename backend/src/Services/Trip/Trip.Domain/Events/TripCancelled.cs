using Trip.Domain.Enums;

namespace Trip.Domain.Events;

public sealed record TripCancelled(
    long TripId,
    CancelledBy CancelledBy,
    string? Reason,
    DateTime CancelledAt) : IDomainEvent;
