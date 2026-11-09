namespace Payment.Presentation.Contracts.Responses;

public sealed record ProcessPaymentResponse(
    long PaymentId,
    string Status,
    DateTimeOffset UpdatedAt
);

