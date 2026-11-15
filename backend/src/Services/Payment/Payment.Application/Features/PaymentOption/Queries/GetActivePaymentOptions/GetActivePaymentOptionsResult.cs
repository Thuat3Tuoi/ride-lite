namespace Payment.Application.Features.PaymentOption.Queries.GetActivePaymentOptions;

public sealed record GetActivePaymentOptionsResult(
    IReadOnlyList<PaymentOptionItemResult> Items
);
