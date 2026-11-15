namespace Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByRefundId;

public sealed record GetLedgerEntriesByRefundIdResult(
    IReadOnlyList<LedgerEntryRefundItemResult> Items
);
