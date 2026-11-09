namespace Payment.Application.Features.Refund.Queries.GetRefundsByPaymentId;

public sealed record RefundItemResult(
    long Id,
    long PaymentId,
    string? ProviderRefundId,
    decimal Amount,
    string? Reason,
    string Status,
    string IdempotencyKey,
    DateTimeOffset? RefundedAt,
    DateTimeOffset CreatedAt
);

public sealed record GetRefundsByPaymentIdResult(
    IReadOnlyList<RefundItemResult> Refunds
);

