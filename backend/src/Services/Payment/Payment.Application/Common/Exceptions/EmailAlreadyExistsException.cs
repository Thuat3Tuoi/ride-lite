namespace Payment.Application.Common.Exceptions;

public sealed class EmailAlreadyExistsException : Exception
{
    public EmailAlreadyExistsException()
        : base("Email is already registered.")
    {
    }
}