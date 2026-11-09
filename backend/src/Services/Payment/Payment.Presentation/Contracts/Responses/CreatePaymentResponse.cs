namespace Payment.Presentation.Contracts.Responses;

public sealed record CreatePaymentResponse(
    long Id,
    long TripId,
    long PaymentOptionId,
    decimal Amount,
    string Status,
    string? IdempotencyKey,
    DateTimeOffset CreatedAt
);

