using Payment.Application.Abstractions.Data;

namespace Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByPaymentId;

public sealed class GetLedgerEntriesByPaymentIdHandler
{
    private readonly ILedgerEntryRepository _ledgerEntryRepository;

    public GetLedgerEntriesByPaymentIdHandler(ILedgerEntryRepository ledgerEntryRepository)
    {
        _ledgerEntryRepository = ledgerEntryRepository;
    }

    public async Task<GetLedgerEntriesByPaymentIdResult> HandleAsync(
        GetLedgerEntriesByPaymentIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var entries = await _ledgerEntryRepository.GetByPaymentIdAsync(
            query.PaymentId,
            cancellationToken
        );

        var items = entries.Select(e => new LedgerEntryPaymentItemResult(
            e.Id,
            e.PaymentId,
            e.RefundId,
            e.EntryType.ToString(),
            e.EntryRef,
            e.DebitAccount,
            e.CreditAccount,
            e.Amount,
            e.CreatedAt
        )).ToList();

        return new GetLedgerEntriesByPaymentIdResult(items);
    }
}

