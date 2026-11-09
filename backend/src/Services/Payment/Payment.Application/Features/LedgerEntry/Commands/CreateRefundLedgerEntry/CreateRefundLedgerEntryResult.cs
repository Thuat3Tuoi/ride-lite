namespace Payment.Application.Features.LedgerEntry.Commands.CreateRefundLedgerEntry;

public sealed record CreateRefundLedgerEntryResult(
    long Id,
    long PaymentId,
    long RefundId,
    string EntryType,
    string EntryRef,
    string DebitAccount,
    string CreditAccount,
    decimal Amount,
    DateTimeOffset CreatedAt
);

