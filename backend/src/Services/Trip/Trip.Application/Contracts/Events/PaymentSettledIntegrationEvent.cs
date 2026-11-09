namespace Trip.Application.Contracts.Events;

public sealed record PaymentSettledIntegrationEvent(
    long TripId,
    string IdempotencyKey);
