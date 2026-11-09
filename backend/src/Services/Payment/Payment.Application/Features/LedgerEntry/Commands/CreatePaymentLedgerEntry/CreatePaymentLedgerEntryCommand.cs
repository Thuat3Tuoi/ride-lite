namespace Payment.Application.Features.LedgerEntry.Commands.CreatePaymentLedgerEntry;

public sealed record CreatePaymentLedgerEntryCommand(
    long PaymentId,
    decimal Amount,
    string DebitAccount,
    string CreditAccount
);

