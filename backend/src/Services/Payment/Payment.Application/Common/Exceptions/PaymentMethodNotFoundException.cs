namespace Payment.Application.Common.Exceptions;

public sealed class PaymentMethodNotFoundException : Exception
{
    public PaymentMethodNotFoundException(long id)
        : base($"Payment method with Id '{id}' was not found.")
    {
    }
}

