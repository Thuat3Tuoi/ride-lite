namespace Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionById;

public sealed record GetPaymentOptionByIdResult(
    long Id,
    long MethodId,
    long ProviderId,
    string Code,
    string Name,
    bool Active
);

