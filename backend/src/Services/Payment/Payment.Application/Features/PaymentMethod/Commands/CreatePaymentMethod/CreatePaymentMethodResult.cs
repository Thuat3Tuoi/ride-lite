namespace Payment.Application.Features.PaymentMethod.Commands.CreatePaymentMethod;

public sealed record CreatePaymentMethodResult(
    long Id,
    string Code,
    string Name,
    bool Active
);

