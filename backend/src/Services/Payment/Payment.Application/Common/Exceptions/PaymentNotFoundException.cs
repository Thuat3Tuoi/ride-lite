namespace Payment.Application.Common.Exceptions;

public sealed class PaymentNotFoundException : Exception
{
    public PaymentNotFoundException(long paymentId)
        : base($"Payment with Id '{paymentId}' was not found.")
    {
    }
}

