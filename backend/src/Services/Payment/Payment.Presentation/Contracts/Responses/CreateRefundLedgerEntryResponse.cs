namespace Payment.Presentation.Contracts.Responses;

public sealed record CreateRefundLedgerEntryResponse(
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

