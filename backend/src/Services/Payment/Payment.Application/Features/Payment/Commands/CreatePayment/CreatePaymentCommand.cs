namespace Payment.Application.Features.Payment.Commands.CreatePayment;

public sealed record CreatePaymentCommand(
    long TripId,
    long PaymentOptionId,
    decimal Amount,
    string? IdempotencyKey = null,
    long? ProviderId = null
);

