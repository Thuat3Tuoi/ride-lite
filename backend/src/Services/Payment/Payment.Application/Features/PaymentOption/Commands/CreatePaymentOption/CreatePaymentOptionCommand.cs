namespace Payment.Application.Features.PaymentOption.Commands.CreatePaymentOption;

public sealed record CreatePaymentOptionCommand(
    long MethodId,
    long ProviderId,
    string Code,
    string Name,
    bool Active = true
);

