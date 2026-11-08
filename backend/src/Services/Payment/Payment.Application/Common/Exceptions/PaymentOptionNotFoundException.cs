namespace Payment.Application.Common.Exceptions;

public sealed class PaymentOptionNotFoundException : Exception
{
    public PaymentOptionNotFoundException(long id)
        : base($"Payment option with Id '{id}' was not found.")
    {
    }
}

