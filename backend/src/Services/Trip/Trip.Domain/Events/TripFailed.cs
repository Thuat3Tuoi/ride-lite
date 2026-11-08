namespace Trip.Domain.Events;

public sealed record TripFailed(
    long TripId,
    string Reason,
    DateTime FailedAt) : IDomainEvent;
