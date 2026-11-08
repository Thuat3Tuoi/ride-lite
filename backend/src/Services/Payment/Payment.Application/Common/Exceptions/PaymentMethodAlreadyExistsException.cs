namespace Payment.Application.Common.Exceptions;

public sealed class PaymentMethodAlreadyExistsException : Exception
{
    public PaymentMethodAlreadyExistsException(string code)
        : base($"Payment method with code '{code}' already exists.")
    {
    }
}

