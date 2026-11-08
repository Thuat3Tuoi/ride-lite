using Payment.Domain.Entities;

namespace Payment.Application.Abstractions.Data;

public interface ILedgerEntryRepository
{
    Task<LedgerEntry?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LedgerEntry>> GetByPaymentIdAsync(
        long paymentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LedgerEntry>> GetByRefundIdAsync(
        long refundId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEntryRefAsync(
        string entryRef,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        LedgerEntry ledgerEntry,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}

