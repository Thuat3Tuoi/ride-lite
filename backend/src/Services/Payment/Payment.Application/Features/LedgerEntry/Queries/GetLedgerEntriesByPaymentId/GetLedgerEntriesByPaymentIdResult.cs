namespace Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByPaymentId;

public sealed record LedgerEntryPaymentItemResult(
    long Id,
    long PaymentId,
    long? RefundId,
    string EntryType,
    string EntryRef,
    string DebitAccount,
    string CreditAccount,
    decimal Amount,
    DateTimeOffset CreatedAt
);

public sealed record GetLedgerEntriesByPaymentIdResult(
    IReadOnlyList<LedgerEntryPaymentItemResult> Items
);

