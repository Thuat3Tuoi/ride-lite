namespace Payment.Application.Features.Refund.Commands.CompleteRefund;

public sealed record CompleteRefundCommand(
    long RefundId,
    string ProviderRefundId
);

