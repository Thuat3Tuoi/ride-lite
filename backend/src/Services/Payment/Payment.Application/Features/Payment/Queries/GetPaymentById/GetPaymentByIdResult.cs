namespace Payment.Application.Features.Payment.Queries.GetPaymentById;

public sealed record GetPaymentByIdResult(
    long Id,
    long TripId,
    long PaymentOptionId,
    long? ProviderId,
    string? ProviderTransactionId,
    string? GatewayToken,
    decimal Amount,
    string? IdempotencyKey,
    string Status,
    DateTimeOffset? PaidAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

