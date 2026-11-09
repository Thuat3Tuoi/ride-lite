namespace Payment.Presentation.Contracts.Responses;

public sealed record CreateRefundResponse(
    long Id,
    long PaymentId,
    decimal Amount,
    string Status,
    string IdempotencyKey,
    string? Reason,
    DateTimeOffset CreatedAt
);

