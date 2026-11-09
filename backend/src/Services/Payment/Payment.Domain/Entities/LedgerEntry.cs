using Payment.Domain.Enumerators;

namespace Payment.Domain.Entities;

public sealed class LedgerEntry
{
    public long Id { get; private set; }
    public long PaymentId { get; private set; }
    public long? RefundId { get; private set; }
    public LedgerEntryType EntryType { get; private set; }
    public string EntryRef { get; private set; } = string.Empty;
    public string DebitAccount { get; private set; } = string.Empty;
    public string CreditAccount { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public Payment? Payment { get; private set; }
    public Refund? Refund { get; private set; }

    private LedgerEntry() { }

    public static LedgerEntry CreatePaymentEntry(
        long paymentId,
        decimal amount,
        string debitAccount,
        string creditAccount)
    {
        return Create(
            paymentId: paymentId,
            refundId: null,
            entryType: LedgerEntryType.Payment,
            entryRef: $"PAYMENT:{paymentId}",
            debitAccount: debitAccount,
            creditAccount: creditAccount,
            amount: amount);
    }

    public static LedgerEntry CreateRefundEntry(
        long paymentId,
        long refundId,
        decimal amount,
        string debitAccount,
        string creditAccount)
    {
        return Create(
            paymentId: paymentId,
            refundId: refundId,
            entryType: LedgerEntryType.Refund,
            entryRef: $"REFUND:{refundId}",
            debitAccount: debitAccount,
            creditAccount: creditAccount,
            amount: amount);
    }

    private static LedgerEntry Create(
        long paymentId,
        long? refundId,
        LedgerEntryType entryType,
        string entryRef,
        string debitAccount,
        string creditAccount,
        decimal amount)
    {
        if (paymentId <= 0)
            throw new ArgumentException("PaymentId must be a positive value.");

        if (entryType == LedgerEntryType.Refund && refundId is null)
            throw new ArgumentException("RefundId is required for a REFUND ledger entry.");

        if (entryType == LedgerEntryType.Payment && refundId is not null)
            throw new ArgumentException("RefundId must be null for a PAYMENT ledger entry.");

        if (string.IsNullOrWhiteSpace(entryRef))
            throw new ArgumentException("EntryRef cannot be empty.");

        if (entryRef.Length > 128)
            throw new ArgumentException("EntryRef cannot exceed 128 characters.");

        if (string.IsNullOrWhiteSpace(debitAccount))
            throw new ArgumentException("Debit account cannot be empty.");

        debitAccount = debitAccount.Trim();

        if (debitAccount.Length > 50)
            throw new ArgumentException("Debit account cannot exceed 50 characters.");

        if (string.IsNullOrWhiteSpace(creditAccount))
            throw new ArgumentException("Credit account cannot be empty.");

        creditAccount = creditAccount.Trim();

        if (creditAccount.Length > 50)
            throw new ArgumentException("Credit account cannot exceed 50 characters.");

        if (amount <= 0)
            throw new ArgumentException("Ledger entry amount must be greater than zero.");

        return new LedgerEntry
        {
            PaymentId = paymentId,
            RefundId = refundId,
            EntryType = entryType,
            EntryRef = entryRef,
            DebitAccount = debitAccount,
            CreditAccount = creditAccount,
            Amount = amount,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
