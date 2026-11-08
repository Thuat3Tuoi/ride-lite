namespace Payment.Application.Features.PaymentMethod.Commands.ActivatePaymentMethod;

public sealed record ActivatePaymentMethodResult(
    long Id,
    bool Active
);

