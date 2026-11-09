namespace Payment.Application.Features.PaymentMethod.Commands.CreatePaymentMethod;

public sealed record CreatePaymentMethodCommand(
    string Code,
    string Name,
    bool Active = true
);

