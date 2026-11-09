using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Entities;
using Payment.Infrastructure.Data.DataContext;

namespace Payment.Infrastructure.Data.Repositories;

public sealed class LedgerEntryRepository : ILedgerEntryRepository
{
    private readonly ApplicationDbContext _context;

    public LedgerEntryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LedgerEntry?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.LedgerEntries
            .Include(l => l.Payment)
            .Include(l => l.Refund)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<LedgerEntry>> GetByPaymentIdAsync(
        long paymentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.LedgerEntries
            .Where(l => l.PaymentId == paymentId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LedgerEntry>> GetByRefundIdAsync(
        long refundId,
        CancellationToken cancellationToken = default)
    {
        return await _context.LedgerEntries
            .Where(l => l.RefundId == refundId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEntryRefAsync(
        string entryRef,
        CancellationToken cancellationToken = default)
    {
        return await _context.LedgerEntries.AnyAsync(
            l => l.EntryRef == entryRef,
            cancellationToken
        );
    }

    public async Task AddAsync(
        LedgerEntry ledgerEntry,
        CancellationToken cancellationToken = default)
    {
        await _context.LedgerEntries.AddAsync(
            ledgerEntry,
            cancellationToken
        );
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(
            cancellationToken
        );
    }
}

