using Microsoft.EntityFrameworkCore;
using Trip.Application.Abstractions.Data;
using Trip.Domain.Entities;
using Trip.Infrastructure.Data.DataContext;

namespace Trip.Infrastructure.Data.Repositories;

public sealed class RiderRepository : IRiderRepository
{
    private readonly ApplicationDbContext _context;

    public RiderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Rider?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Riders
            .Include(r => r.Trips)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Rider?> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Riders
            .Include(r => r.Trips)
            .FirstOrDefaultAsync(r => r.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<bool> ExistsByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Riders
            .AnyAsync(r => r.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Rider>> GetAllAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await _context.Riders
            .AsNoTracking()
            .OrderBy(r => r.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Rider rider,
        CancellationToken cancellationToken = default)
    {
        await _context.Riders.AddAsync(rider, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}

