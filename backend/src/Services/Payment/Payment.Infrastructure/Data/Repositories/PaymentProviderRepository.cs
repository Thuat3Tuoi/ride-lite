using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Entities;
using Payment.Infrastructure.Data.DataContext;

namespace Payment.Infrastructure.Data.Repositories;

public sealed class PaymentProviderRepository : IPaymentProviderRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentProviderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentProvider?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentProviders
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PaymentProvider?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await _context.PaymentProviders
            .FirstOrDefaultAsync(p => p.Code == normalizedCode, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await _context.PaymentProviders.AnyAsync(
            p => p.Code == normalizedCode,
            cancellationToken
        );
    }

    public async Task<IReadOnlyList<PaymentProvider>> GetAllActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentProviders
            .Where(p => p.Active)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        PaymentProvider paymentProvider,
        CancellationToken cancellationToken = default)
    {
        await _context.PaymentProviders.AddAsync(
            paymentProvider,
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

