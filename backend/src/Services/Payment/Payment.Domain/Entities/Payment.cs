using Payment.Domain.Enumerators;

namespace Payment.Domain.Entities;

public sealed class Payment
{
    public long Id { get; private set; }
    public long TripId { get; private set; }
    public long PaymentOptionId { get; private set; }
    public long? ProviderId { get; private set; }
    public string? ProviderTransactionId { get; private set; }
    public string? GatewayToken { get; private set; }
    public decimal Amount { get; private set; }
    public string? IdempotencyKey { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public PaymentOption? Option { get; private set; }
    public PaymentProvider? Provider { get; private set; }
    public IReadOnlyCollection<Refund> Refunds => _refunds.AsReadOnly();
    private readonly List<Refund> _refunds = [];

    private Payment() { }

    public static Payment Create(
        long tripId,
        long paymentOptionId,
        decimal amount,
        string? idempotencyKey = null,
        long? providerId = null)
    {
        if (tripId <= 0)
            throw new ArgumentException("TripId must be a positive value.");

        if (paymentOptionId <= 0)
            throw new ArgumentException("PaymentOptionId must be a positive value.");

        if (amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.");

        if (idempotencyKey is not null)
        {
            idempotencyKey = idempotencyKey.Trim();

            if (idempotencyKey.Length > 128)
                throw new ArgumentException("IdempotencyKey cannot exceed 128 characters.");
        }

        var now = DateTimeOffset.UtcNow;

        return new Payment
        {
            TripId = tripId,
            PaymentOptionId = paymentOptionId,
            ProviderId = providerId,
            Amount = amount,
            IdempotencyKey = idempotencyKey,
            Status = PaymentStatus.Pending,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void MarkSucceeded(
        string providerTransactionId,
        string? gatewayToken = null,
        long? resolvedProviderId = null)
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.Processing)
            throw new InvalidOperationException($"Cannot mark a payment as succeeded when status is '{Status}'.");

        if (string.IsNullOrWhiteSpace(providerTransactionId))
            throw new ArgumentException("Provider transaction ID cannot be empty.");

        providerTransactionId = providerTransactionId.Trim();

        if (providerTransactionId.Length > 128)
            throw new ArgumentException("Provider transaction ID cannot exceed 128 characters.");

        if (gatewayToken is not null && gatewayToken.Length > 255)
            throw new ArgumentException("Gateway token cannot exceed 255 characters.");

        ProviderTransactionId = providerTransactionId;
        GatewayToken = gatewayToken;
        ProviderId = resolvedProviderId ?? ProviderId;
        Status = PaymentStatus.Succeeded;
        PaidAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkProcessing()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot mark a payment as processing when status is '{Status}'.");

        Status = PaymentStatus.Processing;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkFailed()
    {
        if (Status is PaymentStatus.Succeeded or PaymentStatus.Canceled)
            throw new InvalidOperationException($"Cannot mark a payment as failed when status is '{Status}'.");

        Status = PaymentStatus.Failed;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (Status is PaymentStatus.Succeeded or PaymentStatus.Failed)
            throw new InvalidOperationException($"Cannot cancel a payment when status is '{Status}'.");

        Status = PaymentStatus.Canceled;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
