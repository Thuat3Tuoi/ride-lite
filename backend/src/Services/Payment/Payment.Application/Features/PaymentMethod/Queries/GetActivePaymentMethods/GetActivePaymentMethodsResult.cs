namespace Payment.Application.Features.PaymentMethod.Queries.GetActivePaymentMethods;

public sealed record PaymentMethodItemResult(
    long Id,
    string Code,
    string Name,
    bool Active
);

public sealed record GetActivePaymentMethodsResult(
    IReadOnlyList<PaymentMethodItemResult> Items
);

