namespace Payment.Presentation.Contracts.Responses;

public sealed record PaymentResponse(
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

