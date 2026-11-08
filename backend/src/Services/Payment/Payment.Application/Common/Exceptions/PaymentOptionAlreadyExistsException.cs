namespace Payment.Application.Common.Exceptions;

public sealed class PaymentOptionAlreadyExistsException : Exception
{
    public PaymentOptionAlreadyExistsException(string code)
        : base($"Payment option with code '{code}' already exists.")
    {
    }
}

