namespace Payment.Domain.Enumerators;

public enum PaymentStatus
{
    Pending,
    Processing,
    Succeeded,
    Failed,
    Canceled
}
