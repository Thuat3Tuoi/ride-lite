using Payment.Domain.Entities;
using Payment.Domain.Enumerators;
using Xunit;

namespace Payment.Domain.Tests;

public class PaymentTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreatePayment()
    {
        var tripId = 100L;
        var paymentOptionId = 1L;
        var amount = 50000m;
        var idempotencyKey = "  key-12345  ";
        var providerId = 5L;

        var payment = Entities.Payment.Create(
            tripId,
            paymentOptionId,
            amount,
            idempotencyKey,
            providerId);

        Assert.Equal(tripId, payment.TripId);
        Assert.Equal(paymentOptionId, payment.PaymentOptionId);
        Assert.Equal(amount, payment.Amount);
        Assert.Equal("key-12345", payment.IdempotencyKey);
        Assert.Equal(providerId, payment.ProviderId);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Null(payment.PaidAt);
        Assert.NotEqual(default, payment.CreatedAt);
        Assert.NotEqual(default, payment.UpdatedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidTripId_ShouldThrow(long invalidTripId)
    {
        Assert.Throws<ArgumentException>(() =>
            Entities.Payment.Create(invalidTripId, 1L, 50000m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidPaymentOptionId_ShouldThrow(long invalidOptionId)
    {
        Assert.Throws<ArgumentException>(() =>
            Entities.Payment.Create(100L, invalidOptionId, 50000m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50000)]
    public void Create_WithInvalidAmount_ShouldThrow(decimal invalidAmount)
    {
        Assert.Throws<ArgumentException>(() =>
            Entities.Payment.Create(100L, 1L, invalidAmount));
    }

    [Fact]
    public void Create_WithExcessiveIdempotencyKey_ShouldThrow()
    {
        var longKey = new string('a', 129);

        Assert.Throws<ArgumentException>(() =>
            Entities.Payment.Create(100L, 1L, 50000m, longKey));
    }

    [Fact]
    public void MarkProcessing_WhenPending_ShouldSetStatusToProcessing()
    {
        var payment = Entities.Payment.Create(100L, 1L, 50000m);

        payment.MarkProcessing();

        Assert.Equal(PaymentStatus.Processing, payment.Status);
    }

    [Fact]
    public void MarkProcessing_WhenNotPending_ShouldThrow()
    {
        var payment = Entities.Payment.Create(100L, 1L, 50000m);
        payment.MarkProcessing();

        Assert.Throws<InvalidOperationException>(() => payment.MarkProcessing());
    }

    [Fact]
    public void MarkSucceeded_WhenPendingOrProcessing_ShouldSetStatusToSucceeded()
    {
        var payment = Entities.Payment.Create(100L, 1L, 50000m);

        payment.MarkSucceeded("tx-001", "tok-xyz", 10L);

        Assert.Equal(PaymentStatus.Succeeded, payment.Status);
        Assert.Equal("tx-001", payment.ProviderTransactionId);
        Assert.Equal("tok-xyz", payment.GatewayToken);
        Assert.Equal(10L, payment.ProviderId);
        Assert.NotNull(payment.PaidAt);
    }

    [Fact]
    public void MarkSucceeded_WhenAlreadySucceeded_ShouldThrow()
    {
        var payment = Entities.Payment.Create(100L, 1L, 50000m);
        payment.MarkSucceeded("tx-001");

        Assert.Throws<InvalidOperationException>(() => payment.MarkSucceeded("tx-002"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void MarkSucceeded_WithEmptyProviderTransactionId_ShouldThrow(string invalidTxId)
    {
        var payment = Entities.Payment.Create(100L, 1L, 50000m);

        Assert.Throws<ArgumentException>(() => payment.MarkSucceeded(invalidTxId));
    }

    [Fact]
    public void MarkSucceeded_WithExcessiveProviderTransactionId_ShouldThrow()
    {
        var payment = Entities.Payment.Create(100L, 1L, 50000m);
        var longTxId = new string('a', 129);

        Assert.Throws<ArgumentException>(() => payment.MarkSucceeded(longTxId));
    }

    [Fact]
    public void MarkSucceeded_WithExcessiveGatewayToken_ShouldThrow()
    {
        var payment = Entities.Payment.Create(100L, 1L, 50000m);
        var longToken = new string('a', 256);

        Assert.Throws<ArgumentException>(() => payment.MarkSucceeded("tx-001", longToken));
    }

    [Fact]
    public void MarkFailed_WhenPendingOrProcessing_ShouldSetStatusToFailed()
    {
        var payment = Entities.Payment.Create(100L, 1L, 50000m);

        payment.MarkFailed();

        Assert.Equal(PaymentStatus.Failed, payment.Status);
    }

    [Fact]
    public void MarkFailed_WhenAlreadySucceeded_ShouldThrow()
    {
        var payment = Entities.Payment.Create(100L, 1L, 50000m);
        payment.MarkSucceeded("tx-001");

        Assert.Throws<InvalidOperationException>(() => payment.MarkFailed());
    }

    [Fact]
    public void Cancel_WhenPending_ShouldSetStatusToCanceled()
    {
        var payment = Entities.Payment.Create(100L, 1L, 50000m);

        payment.Cancel();

        Assert.Equal(PaymentStatus.Canceled, payment.Status);
    }

    [Fact]
    public void Cancel_WhenAlreadyFailed_ShouldThrow()
    {
        var payment = Entities.Payment.Create(100L, 1L, 50000m);
        payment.MarkFailed();

        Assert.Throws<InvalidOperationException>(() => payment.Cancel());
    }
}

