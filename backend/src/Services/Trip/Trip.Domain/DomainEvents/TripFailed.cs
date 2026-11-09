namespace Trip.Domain.DomainEvents;

public sealed record TripFailed(
    long TripId,
    string Reason,
    DateTime FailedAt) : IDomainEvent;
