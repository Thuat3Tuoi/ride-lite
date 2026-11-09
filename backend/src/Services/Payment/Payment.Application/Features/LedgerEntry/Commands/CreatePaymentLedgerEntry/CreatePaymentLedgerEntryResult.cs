namespace Payment.Application.Features.LedgerEntry.Commands.CreatePaymentLedgerEntry;

public sealed record CreatePaymentLedgerEntryResult(
    long Id,
    long PaymentId,
    string EntryType,
    string EntryRef,
    string DebitAccount,
    string CreditAccount,
    decimal Amount,
    DateTimeOffset CreatedAt
);

