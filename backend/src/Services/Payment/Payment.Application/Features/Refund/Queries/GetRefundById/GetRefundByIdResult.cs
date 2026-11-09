namespace Payment.Application.Features.Refund.Queries.GetRefundById;

public sealed record GetRefundByIdResult(
    long Id,
    long PaymentId,
    string? ProviderRefundId,
    decimal Amount,
    string? Reason,
    string Status,
    string IdempotencyKey,
    DateTimeOffset? RefundedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

