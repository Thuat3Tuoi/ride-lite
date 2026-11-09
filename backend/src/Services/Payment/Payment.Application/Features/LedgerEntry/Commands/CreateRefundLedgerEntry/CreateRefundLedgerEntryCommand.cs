namespace Payment.Application.Features.LedgerEntry.Commands.CreateRefundLedgerEntry;

public sealed record CreateRefundLedgerEntryCommand(
    long PaymentId,
    long RefundId,
    decimal Amount,
    string DebitAccount,
    string CreditAccount
);

