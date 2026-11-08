namespace Payment.Application.Common.Exceptions;

public sealed class RefundNotFoundException : Exception
{
    public RefundNotFoundException(long refundId)
        : base($"Refund with Id '{refundId}' was not found.")
    {
    }
}

