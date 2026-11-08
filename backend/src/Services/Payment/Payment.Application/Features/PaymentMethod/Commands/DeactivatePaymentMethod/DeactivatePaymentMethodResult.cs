namespace Payment.Application.Features.PaymentMethod.Commands.DeactivatePaymentMethod;

public sealed record DeactivatePaymentMethodResult(
    long Id,
    bool Active
);

