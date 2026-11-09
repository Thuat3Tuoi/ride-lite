namespace Payment.Presentation.Contracts.Responses;

public sealed record PaymentOptionStatusResponse(
    long Id,
    bool Active
);

