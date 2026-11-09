namespace Payment.Application.Features.PaymentOption.Commands.DeactivatePaymentOption;

public sealed record DeactivatePaymentOptionResult(
    long Id,
    bool Active
);

