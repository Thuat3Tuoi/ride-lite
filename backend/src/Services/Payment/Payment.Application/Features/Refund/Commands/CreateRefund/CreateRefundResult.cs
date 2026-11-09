namespace Payment.Application.Features.Refund.Commands.CreateRefund;

public sealed record CreateRefundResult(
    long Id,
    long PaymentId,
    decimal Amount,
    string Status,
    string IdempotencyKey,
    string? Reason,
    DateTimeOffset CreatedAt
);

