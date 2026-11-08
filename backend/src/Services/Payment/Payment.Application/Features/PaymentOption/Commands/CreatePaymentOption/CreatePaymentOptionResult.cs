namespace Payment.Application.Features.PaymentOption.Commands.CreatePaymentOption;

public sealed record CreatePaymentOptionResult(
    long Id,
    long MethodId,
    long ProviderId,
    string Code,
    string Name,
    bool Active
);

