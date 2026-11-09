using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Entities;
using Payment.Infrastructure.Data.DataContext;

namespace Payment.Infrastructure.Data.Repositories;

public sealed class RefundRepository : IRefundRepository
{
    private readonly ApplicationDbContext _context;

    public RefundRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Refund?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Refunds
            .Include(r => r.Payment)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Refund>> GetByPaymentIdAsync(
        long paymentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Refunds
            .Where(r => r.PaymentId == paymentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Refund?> GetByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        return await _context.Refunds
            .FirstOrDefaultAsync(r => r.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public async Task<bool> ExistsByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        return await _context.Refunds.AnyAsync(
            r => r.IdempotencyKey == idempotencyKey,
            cancellationToken
        );
    }

    public async Task AddAsync(
        Refund refund,
        CancellationToken cancellationToken = default)
    {
        await _context.Refunds.AddAsync(
            refund,
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

