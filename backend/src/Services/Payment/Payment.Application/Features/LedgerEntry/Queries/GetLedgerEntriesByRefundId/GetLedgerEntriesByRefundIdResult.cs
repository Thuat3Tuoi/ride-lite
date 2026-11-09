namespace Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByRefundId;

public sealed record LedgerEntryRefundItemResult(
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

public sealed record GetLedgerEntriesByRefundIdResult(
    IReadOnlyList<LedgerEntryRefundItemResult> Items
);

