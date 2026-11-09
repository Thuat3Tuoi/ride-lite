namespace Payment.Presentation.Contracts.Responses;

public sealed record LedgerEntryResponse(
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

