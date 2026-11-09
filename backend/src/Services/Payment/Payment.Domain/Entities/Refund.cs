using Payment.Domain.Enumerators;

namespace Payment.Domain.Entities;

public sealed class Refund
{
    public long Id { get; private set; }
    public long PaymentId { get; private set; }
    public string? ProviderRefundId { get; private set; }
    public decimal Amount { get; private set; }
    public string? Reason { get; private set; }
    public RefundStatus Status { get; private set; }
    public string IdempotencyKey { get; private set; } = string.Empty;
    public DateTimeOffset? RefundedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public Payment? Payment { get; private set; }

    private Refund() { }

    public static Refund Create(
        long paymentId,
        decimal amount,
        string idempotencyKey,
        string? reason = null)
    {
        if (paymentId <= 0)
            throw new ArgumentException("PaymentId must be a positive value.");

        if (amount <= 0)
            throw new ArgumentException("Refund amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException("IdempotencyKey cannot be empty.");

        idempotencyKey = idempotencyKey.Trim();

        if (idempotencyKey.Length > 128)
            throw new ArgumentException("IdempotencyKey cannot exceed 128 characters.");

        if (reason is not null)
        {
            reason = reason.Trim();

            if (reason.Length > 255)
                throw new ArgumentException("Reason cannot exceed 255 characters.");
        }

        var now = DateTimeOffset.UtcNow;

        return new Refund
        {
            PaymentId = paymentId,
            Amount = amount,
            IdempotencyKey = idempotencyKey,
            Reason = reason,
            Status = RefundStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void MarkProcessing()
    {
        if (Status != RefundStatus.Pending)
            throw new InvalidOperationException($"Cannot mark a refund as processing when status is '{Status}'.");

        Status = RefundStatus.Processing;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkSucceeded(string providerRefundId)
    {
        if (Status != RefundStatus.Processing)
            throw new InvalidOperationException($"Cannot mark a refund as succeeded when status is '{Status}'.");

        if (string.IsNullOrWhiteSpace(providerRefundId))
            throw new ArgumentException("Provider refund ID cannot be empty.");

        providerRefundId = providerRefundId.Trim();

        if (providerRefundId.Length > 128)
            throw new ArgumentException("Provider refund ID cannot exceed 128 characters.");

        ProviderRefundId = providerRefundId;
        Status = RefundStatus.Succeeded;
        RefundedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkFailed()
    {
        if (Status is RefundStatus.Succeeded)
            throw new InvalidOperationException("Cannot mark an already succeeded refund as failed.");

        Status = RefundStatus.Failed;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
