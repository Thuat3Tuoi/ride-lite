namespace Payment.Application.Features.Payment.Commands.CompletePayment;

public sealed record CompletePaymentCommand(
    long PaymentId,
    string ProviderTransactionId,
    string? GatewayToken = null,
    long? ResolvedProviderId = null
);

