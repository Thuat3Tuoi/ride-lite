namespace Payment.Application.Features.PaymentProvider.Commands.DeactivatePaymentProvider;

public sealed record DeactivatePaymentProviderResult(
    long Id,
    bool Active
);

