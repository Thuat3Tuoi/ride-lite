namespace Payment.Application.Features.PaymentProvider.Commands.ActivatePaymentProvider;

public sealed record ActivatePaymentProviderResult(
    long Id,
    bool Active
);

