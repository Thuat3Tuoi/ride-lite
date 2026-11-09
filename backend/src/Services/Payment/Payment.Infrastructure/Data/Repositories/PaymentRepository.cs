using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions.Data;
using Payment.Infrastructure.Data.DataContext;
using PaymentEntity = Payment.Domain.Entities.Payment;

namespace Payment.Infrastructure.Data.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentEntity?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Option)
            .Include(p => p.Provider)
            .Include(p => p.Refunds)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PaymentEntity?> GetByTripIdAsync(
        long tripId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Option)
            .Include(p => p.Provider)
            .Include(p => p.Refunds)
            .FirstOrDefaultAsync(p => p.TripId == tripId, cancellationToken);
    }

    public async Task<PaymentEntity?> GetByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public async Task<bool> ExistsByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments.AnyAsync(
            p => p.IdempotencyKey == idempotencyKey,
            cancellationToken
        );
    }

    public async Task AddAsync(
        PaymentEntity payment,
        CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(
            payment,
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

