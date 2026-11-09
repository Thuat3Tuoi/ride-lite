namespace Payment.Application.Features.Payment.Commands.CreatePayment;

public sealed record CreatePaymentResult(
    long Id,
    long TripId,
    long PaymentOptionId,
    decimal Amount,
    string Status,
    string? IdempotencyKey,
    DateTimeOffset CreatedAt
);

