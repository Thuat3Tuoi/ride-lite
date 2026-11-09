namespace Payment.Presentation.Contracts.Responses;

public sealed record RefundResponse(
    long Id,
    long PaymentId,
    string? ProviderRefundId,
    decimal Amount,
    string? Reason,
    string Status,
    string IdempotencyKey,
    DateTimeOffset? RefundedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt = null
);

