namespace Payment.Application.Features.PaymentOption.Commands.ActivatePaymentOption;

public sealed record ActivatePaymentOptionResult(
    long Id,
    bool Active
);

