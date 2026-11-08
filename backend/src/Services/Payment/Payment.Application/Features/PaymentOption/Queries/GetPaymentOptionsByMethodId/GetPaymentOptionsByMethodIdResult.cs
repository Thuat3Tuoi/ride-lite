namespace Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionsByMethodId;

public sealed record PaymentOptionByMethodItemResult(
    long Id,
    long MethodId,
    long ProviderId,
    string Code,
    string Name,
    bool Active
);

public sealed record GetPaymentOptionsByMethodIdResult(
    IReadOnlyList<PaymentOptionByMethodItemResult> Items
);

