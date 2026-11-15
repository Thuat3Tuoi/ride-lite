namespace Payment.Application.Features.PaymentOption.Queries.GetPaymentOptionsByMethodId;

public sealed record GetPaymentOptionsByMethodIdResult(
    IReadOnlyList<PaymentOptionByMethodItemResult> Items
);
