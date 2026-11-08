using Payment.Domain.Entities;
using Payment.Domain.Enumerators;
using Xunit;

namespace Payment.Domain.Tests;

public class LedgerEntryTests
{
    [Fact]
    public void CreatePaymentEntry_WithValidData_ShouldCreatePaymentLedgerEntry()
    {
        var paymentId = 100L;
        var amount = 50000m;
        var debit = "  USER_WALLET  ";
        var credit = "  DRIVER_WALLET  ";

        var entry = LedgerEntry.CreatePaymentEntry(paymentId, amount, debit, credit);

        Assert.Equal(paymentId, entry.PaymentId);
        Assert.Null(entry.RefundId);
        Assert.Equal(LedgerEntryType.Payment, entry.EntryType);
        Assert.Equal("PAYMENT:100", entry.EntryRef);
        Assert.Equal("USER_WALLET", entry.DebitAccount);
        Assert.Equal("DRIVER_WALLET", entry.CreditAccount);
        Assert.Equal(amount, entry.Amount);
        Assert.NotEqual(default, entry.CreatedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreatePaymentEntry_WithInvalidPaymentId_ShouldThrow(long invalidPaymentId)
    {
        Assert.Throws<ArgumentException>(() =>
            LedgerEntry.CreatePaymentEntry(invalidPaymentId, 50000m, "DEBIT", "CREDIT"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1000)]
    public void CreatePaymentEntry_WithInvalidAmount_ShouldThrow(decimal invalidAmount)
    {
        Assert.Throws<ArgumentException>(() =>
            LedgerEntry.CreatePaymentEntry(100L, invalidAmount, "DEBIT", "CREDIT"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreatePaymentEntry_WithEmptyDebitAccount_ShouldThrow(string invalidDebit)
    {
        Assert.Throws<ArgumentException>(() =>
            LedgerEntry.CreatePaymentEntry(100L, 50000m, invalidDebit, "CREDIT"));
    }

    [Fact]
    public void CreatePaymentEntry_WithExcessiveDebitAccount_ShouldThrow()
    {
        var longAccount = new string('a', 51);

        Assert.Throws<ArgumentException>(() =>
            LedgerEntry.CreatePaymentEntry(100L, 50000m, longAccount, "CREDIT"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreatePaymentEntry_WithEmptyCreditAccount_ShouldThrow(string invalidCredit)
    {
        Assert.Throws<ArgumentException>(() =>
            LedgerEntry.CreatePaymentEntry(100L, 50000m, "DEBIT", invalidCredit));
    }

    [Fact]
    public void CreatePaymentEntry_WithExcessiveCreditAccount_ShouldThrow()
    {
        var longAccount = new string('a', 51);

        Assert.Throws<ArgumentException>(() =>
            LedgerEntry.CreatePaymentEntry(100L, 50000m, "DEBIT", longAccount));
    }

    [Fact]
    public void CreateRefundEntry_WithValidData_ShouldCreateRefundLedgerEntry()
    {
        var paymentId = 100L;
        var refundId = 200L;
        var amount = 25000m;
        var debit = "DRIVER_WALLET";
        var credit = "USER_WALLET";

        var entry = LedgerEntry.CreateRefundEntry(paymentId, refundId, amount, debit, credit);

        Assert.Equal(paymentId, entry.PaymentId);
        Assert.Equal(refundId, entry.RefundId);
        Assert.Equal(LedgerEntryType.Refund, entry.EntryType);
        Assert.Equal("REFUND:200", entry.EntryRef);
        Assert.Equal("DRIVER_WALLET", entry.DebitAccount);
        Assert.Equal("USER_WALLET", entry.CreditAccount);
        Assert.Equal(amount, entry.Amount);
        Assert.NotEqual(default, entry.CreatedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateRefundEntry_WithInvalidPaymentId_ShouldThrow(long invalidPaymentId)
    {
        Assert.Throws<ArgumentException>(() =>
            LedgerEntry.CreateRefundEntry(invalidPaymentId, 200L, 25000m, "DEBIT", "CREDIT"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1000)]
    public void CreateRefundEntry_WithInvalidAmount_ShouldThrow(decimal invalidAmount)
    {
        Assert.Throws<ArgumentException>(() =>
            LedgerEntry.CreateRefundEntry(100L, 200L, invalidAmount, "DEBIT", "CREDIT"));
    }
}

