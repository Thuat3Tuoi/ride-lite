namespace Payment.Application.Features.Payment.Commands.ProcessPayment;

public sealed record ProcessPaymentResult(
    long PaymentId,
    string Status,
    DateTimeOffset UpdatedAt
);

