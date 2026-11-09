namespace Trip.Application.Contracts.Events;

public sealed record PaymentFailedIntegrationEvent(
    long TripId,
    string IdempotencyKey,
    string? Reason);
