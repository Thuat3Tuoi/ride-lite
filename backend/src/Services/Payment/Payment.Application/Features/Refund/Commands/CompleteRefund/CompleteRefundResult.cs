namespace Payment.Application.Features.Refund.Commands.CompleteRefund;

public sealed record CompleteRefundResult(
    long RefundId,
    string Status,
    string ProviderRefundId,
    DateTimeOffset? RefundedAt,
    DateTimeOffset UpdatedAt
);

