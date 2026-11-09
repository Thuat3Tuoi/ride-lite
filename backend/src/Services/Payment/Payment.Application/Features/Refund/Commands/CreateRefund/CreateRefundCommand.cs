namespace Payment.Application.Features.Refund.Commands.CreateRefund;

public sealed record CreateRefundCommand(
    long PaymentId,
    decimal Amount,
    string IdempotencyKey,
    string? Reason = null
);

