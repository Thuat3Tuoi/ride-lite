namespace Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByPaymentId;

public sealed record GetLedgerEntriesByPaymentIdQuery(
    long PaymentId
);

