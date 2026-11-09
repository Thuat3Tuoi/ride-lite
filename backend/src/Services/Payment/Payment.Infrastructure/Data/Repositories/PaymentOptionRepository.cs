using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions.Data;
using Payment.Domain.Entities;
using Payment.Infrastructure.Data.DataContext;

namespace Payment.Infrastructure.Data.Repositories;

public sealed class PaymentOptionRepository : IPaymentOptionRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentOptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentOption?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentOptions
            .Include(o => o.Method)
            .Include(o => o.Provider)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<PaymentOption?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await _context.PaymentOptions
            .Include(o => o.Method)
            .Include(o => o.Provider)
            .FirstOrDefaultAsync(o => o.Code == normalizedCode, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await _context.PaymentOptions.AnyAsync(
            o => o.Code == normalizedCode,
            cancellationToken
        );
    }

    public async Task<IReadOnlyList<PaymentOption>> GetByMethodIdAsync(
        long methodId,
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentOptions
            .Where(o => o.MethodId == methodId)
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentOption>> GetAllActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.PaymentOptions
            .Include(o => o.Method)
            .Include(o => o.Provider)
            .Where(o => o.Active)
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        PaymentOption paymentOption,
        CancellationToken cancellationToken = default)
    {
        await _context.PaymentOptions.AddAsync(
            paymentOption,
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

