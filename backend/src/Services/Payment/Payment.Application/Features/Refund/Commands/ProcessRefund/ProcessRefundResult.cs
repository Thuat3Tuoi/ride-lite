namespace Payment.Application.Features.Refund.Commands.ProcessRefund;

public sealed record ProcessRefundResult(
    long RefundId,
    string Status,
    DateTimeOffset UpdatedAt
);

