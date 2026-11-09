namespace Payment.Application.Features.PaymentMethod.Queries.GetPaymentMethodById;

public sealed record GetPaymentMethodByIdResult(
    long Id,
    string Code,
    string Name,
    bool Active
);

