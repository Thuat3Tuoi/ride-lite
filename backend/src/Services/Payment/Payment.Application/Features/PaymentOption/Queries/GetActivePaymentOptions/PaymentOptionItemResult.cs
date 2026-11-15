namespace Payment.Application.Features.PaymentOption.Queries.GetActivePaymentOptions;

public sealed record PaymentOptionItemResult(
    long Id,
    long MethodId,
    long ProviderId,
    string Code,
    string Name,
    bool Active
);

