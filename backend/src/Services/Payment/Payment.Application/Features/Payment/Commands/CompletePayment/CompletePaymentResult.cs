namespace Payment.Application.Features.Payment.Commands.CompletePayment;

public sealed record CompletePaymentResult(
    long PaymentId,
    string Status,
    string ProviderTransactionId,
    string? GatewayToken,
    DateTimeOffset? PaidAt,
    DateTimeOffset UpdatedAt
);

