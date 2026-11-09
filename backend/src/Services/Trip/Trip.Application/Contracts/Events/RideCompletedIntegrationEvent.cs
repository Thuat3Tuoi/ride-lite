namespace Trip.Application.Contracts.Events;

public sealed record RideCompletedIntegrationEvent(
    long TripId,
    long RiderId,
    decimal Amount,
    string IdempotencyKey,
    DateTime CompletedAt);
