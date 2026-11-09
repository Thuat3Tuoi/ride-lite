namespace Payment.Presentation.Contracts.Responses;

public sealed record PaymentMethodStatusResponse(
    long Id,
    bool Active
);

