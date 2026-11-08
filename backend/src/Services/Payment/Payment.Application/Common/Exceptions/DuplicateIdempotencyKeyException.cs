namespace Payment.Application.Common.Exceptions;

public sealed class DuplicateIdempotencyKeyException : Exception
{
    public DuplicateIdempotencyKeyException(string key)
        : base($"A record with idempotency key '{key}' already exists.")
    {
    }
}

