namespace Payment.Application.Features.PaymentMethod.Queries.GetActivePaymentMethods;

public sealed record GetActivePaymentMethodsResult(
    IReadOnlyList<PaymentMethodItemResult> Items
);
