namespace Payment.Application.Features.Payment.Commands.CancelPayment;

public sealed record CancelPaymentCommand(
    long PaymentId
);

