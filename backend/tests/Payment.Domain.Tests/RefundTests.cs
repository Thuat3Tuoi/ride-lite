using Payment.Domain.Entities;
using Payment.Domain.Enumerators;
using Xunit;

namespace Payment.Domain.Tests;

public class RefundTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateRefund()
    {
        var paymentId = 10L;
        var amount = 25000m;
        var idempotencyKey = "  refund-key-1  ";
        var reason = "  Customer requested cancellation  ";

        var refund = Refund.Create(paymentId, amount, idempotencyKey, reason);

        Assert.Equal(paymentId, refund.PaymentId);
        Assert.Equal(amount, refund.Amount);
        Assert.Equal("refund-key-1", refund.IdempotencyKey);
        Assert.Equal("Customer requested cancellation", refund.Reason);
        Assert.Equal(RefundStatus.Pending, refund.Status);
        Assert.Null(refund.RefundedAt);
        Assert.NotEqual(default, refund.CreatedAt);
        Assert.NotEqual(default, refund.UpdatedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidPaymentId_ShouldThrow(long invalidPaymentId)
    {
        Assert.Throws<ArgumentException>(() =>
            Refund.Create(invalidPaymentId, 25000m, "key-1"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1000)]
    public void Create_WithInvalidAmount_ShouldThrow(decimal invalidAmount)
    {
        Assert.Throws<ArgumentException>(() =>
            Refund.Create(10L, invalidAmount, "key-1"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyIdempotencyKey_ShouldThrow(string invalidKey)
    {
        Assert.Throws<ArgumentException>(() =>
            Refund.Create(10L, 25000m, invalidKey));
    }

    [Fact]
    public void Create_WithExcessiveIdempotencyKey_ShouldThrow()
    {
        var longKey = new string('a', 129);

        Assert.Throws<ArgumentException>(() =>
            Refund.Create(10L, 25000m, longKey));
    }

    [Fact]
    public void Create_WithExcessiveReason_ShouldThrow()
    {
        var longReason = new string('a', 256);

        Assert.Throws<ArgumentException>(() =>
            Refund.Create(10L, 25000m, "key-1", longReason));
    }

    [Fact]
    public void MarkProcessing_WhenPending_ShouldSetStatusToProcessing()
    {
        var refund = Refund.Create(10L, 25000m, "key-1");

        refund.MarkProcessing();

        Assert.Equal(RefundStatus.Processing, refund.Status);
    }

    [Fact]
    public void MarkProcessing_WhenNotPending_ShouldThrow()
    {
        var refund = Refund.Create(10L, 25000m, "key-1");
        refund.MarkProcessing();

        Assert.Throws<InvalidOperationException>(() => refund.MarkProcessing());
    }

    [Fact]
    public void MarkSucceeded_WhenProcessing_ShouldSetStatusToSucceeded()
    {
        var refund = Refund.Create(10L, 25000m, "key-1");
        refund.MarkProcessing();

        refund.MarkSucceeded("ref-tx-999");

        Assert.Equal(RefundStatus.Succeeded, refund.Status);
        Assert.Equal("ref-tx-999", refund.ProviderRefundId);
        Assert.NotNull(refund.RefundedAt);
    }

    [Fact]
    public void MarkSucceeded_WhenNotProcessing_ShouldThrow()
    {
        var refund = Refund.Create(10L, 25000m, "key-1");

        Assert.Throws<InvalidOperationException>(() => refund.MarkSucceeded("ref-tx-999"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void MarkSucceeded_WithEmptyProviderRefundId_ShouldThrow(string invalidId)
    {
        var refund = Refund.Create(10L, 25000m, "key-1");
        refund.MarkProcessing();

        Assert.Throws<ArgumentException>(() => refund.MarkSucceeded(invalidId));
    }

    [Fact]
    public void MarkSucceeded_WithExcessiveProviderRefundId_ShouldThrow()
    {
        var refund = Refund.Create(10L, 25000m, "key-1");
        refund.MarkProcessing();
        var longId = new string('a', 129);

        Assert.Throws<ArgumentException>(() => refund.MarkSucceeded(longId));
    }

    [Fact]
    public void MarkFailed_WhenPendingOrProcessing_ShouldSetStatusToFailed()
    {
        var refund = Refund.Create(10L, 25000m, "key-1");

        refund.MarkFailed();

        Assert.Equal(RefundStatus.Failed, refund.Status);
    }

    [Fact]
    public void MarkFailed_WhenAlreadySucceeded_ShouldThrow()
    {
        var refund = Refund.Create(10L, 25000m, "key-1");
        refund.MarkProcessing();
        refund.MarkSucceeded("ref-tx-999");

        Assert.Throws<InvalidOperationException>(() => refund.MarkFailed());
    }
}

