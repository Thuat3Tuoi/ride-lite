namespace Payment.Application.Features.Refund.Queries.GetRefundsByPaymentId;

public sealed record GetRefundsByPaymentIdQuery(
    long PaymentId
);

