using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Entities;
using Payment.Infrastructure.Data.DataContext;

namespace Payment.Infrastructure.Data.Repositories;

public sealed class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentMethodRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentMethod?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentMethods
            .Include(m => m.Options)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<PaymentMethod?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await _context.PaymentMethods
            .FirstOrDefaultAsync(m => m.Code == normalizedCode, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await _context.PaymentMethods.AnyAsync(
            m => m.Code == normalizedCode,
            cancellationToken
        );
    }

    public async Task<IReadOnlyList<PaymentMethod>> GetAllActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentMethods
            .Where(m => m.Active)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        PaymentMethod paymentMethod,
        CancellationToken cancellationToken = default)
    {
        await _context.PaymentMethods.AddAsync(
            paymentMethod,
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

