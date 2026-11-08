namespace Payment.Application.Common.Exceptions;

public sealed class PaymentProviderNotFoundException : Exception
{
    public PaymentProviderNotFoundException(long id)
        : base($"Payment provider with Id '{id}' was not found.")
    {
    }
}

