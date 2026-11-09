namespace Payment.Presentation.Contracts.Responses;

public sealed record CancelPaymentResponse(
    long PaymentId,
    string Status,
    DateTimeOffset UpdatedAt
);

