using Payment.Application.Abstractions.Data;

namespace Payment.Application.Features.LedgerEntry.Queries.GetLedgerEntriesByRefundId;

public sealed class GetLedgerEntriesByRefundIdHandler
{
    private readonly ILedgerEntryRepository _ledgerEntryRepository;

    public GetLedgerEntriesByRefundIdHandler(ILedgerEntryRepository ledgerEntryRepository)
    {
        _ledgerEntryRepository = ledgerEntryRepository;
    }

    public async Task<GetLedgerEntriesByRefundIdResult> HandleAsync(
        GetLedgerEntriesByRefundIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var entries = await _ledgerEntryRepository.GetByRefundIdAsync(
            query.RefundId,
            cancellationToken
        );

        var items = entries.Select(e => new LedgerEntryRefundItemResult(
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

        return new GetLedgerEntriesByRefundIdResult(items);
    }
}

