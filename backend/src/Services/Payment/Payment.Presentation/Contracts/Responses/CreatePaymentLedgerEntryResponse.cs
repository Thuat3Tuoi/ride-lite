namespace Payment.Presentation.Contracts.Responses;

public sealed record CreatePaymentLedgerEntryResponse(
    long Id,
    long PaymentId,
    string EntryType,
    string EntryRef,
    string DebitAccount,
    string CreditAccount,
    decimal Amount,
    DateTimeOffset CreatedAt
);

