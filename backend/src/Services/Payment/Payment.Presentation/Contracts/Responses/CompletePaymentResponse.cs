namespace Payment.Presentation.Contracts.Responses;

public sealed record CompletePaymentResponse(
    long PaymentId,
    string Status,
    string ProviderTransactionId,
    string? GatewayToken,
    DateTimeOffset? PaidAt,
    DateTimeOffset UpdatedAt
);

