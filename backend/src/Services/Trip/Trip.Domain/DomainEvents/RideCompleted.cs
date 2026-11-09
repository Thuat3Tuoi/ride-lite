namespace Trip.Domain.DomainEvents;

public sealed record RideCompleted(
    long TripId,
    long RiderId,
    decimal Amount,
    string IdempotencyKey,
    DateTime CompletedAt) : IDomainEvent;
