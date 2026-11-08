namespace Trip.Domain.Events;

public sealed record RideCompleted(
    long TripId,
    long RiderId,
    decimal Amount,
    string IdempotencyKey,
    DateTime CompletedAt) : IDomainEvent;
