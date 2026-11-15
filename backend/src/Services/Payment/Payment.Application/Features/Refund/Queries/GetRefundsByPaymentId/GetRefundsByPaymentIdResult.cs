namespace Payment.Application.Features.Refund.Queries.GetRefundsByPaymentId;

public sealed record GetRefundsByPaymentIdResult(
    IReadOnlyList<RefundItemResult> Refunds
);
