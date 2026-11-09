namespace Payment.Application.Features.Payment.Commands.CancelPayment;

public sealed record CancelPaymentResult(
    long PaymentId,
    string Status,
    DateTimeOffset UpdatedAt
);

