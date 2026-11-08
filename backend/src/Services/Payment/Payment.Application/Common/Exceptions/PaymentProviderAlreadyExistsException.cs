namespace Payment.Application.Common.Exceptions;

public sealed class PaymentProviderAlreadyExistsException : Exception
{
    public PaymentProviderAlreadyExistsException(string code)
        : base($"Payment provider with code '{code}' already exists.")
    {
    }
}

