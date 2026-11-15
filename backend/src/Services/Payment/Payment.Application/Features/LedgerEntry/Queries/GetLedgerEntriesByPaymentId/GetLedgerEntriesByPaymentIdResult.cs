namespace Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByPaymentId;

public sealed record GetLedgerEntriesByPaymentIdResult(
    IReadOnlyList<LedgerEntryPaymentItemResult> Items
);
