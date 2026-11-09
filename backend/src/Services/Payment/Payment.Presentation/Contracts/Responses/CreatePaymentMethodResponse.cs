namespace Payment.Presentation.Contracts.Responses;

public sealed record CreatePaymentMethodResponse(
    long Id,
    string Code,
    string Name,
    bool Active
);

