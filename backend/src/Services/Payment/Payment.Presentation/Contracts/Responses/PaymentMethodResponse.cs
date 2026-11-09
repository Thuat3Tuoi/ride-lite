namespace Payment.Presentation.Contracts.Responses;

public sealed record PaymentMethodResponse(
    long Id,
    string Code,
    string Name,
    bool Active
);

