using Microsoft.EntityFrameworkCore;
using Trip.Application.Abstractions.Data;
using Trip.Domain.Enumerators;
using Trip.Infrastructure.Data.DataContext;
using TripEntity = Trip.Domain.Entities.Trip;

namespace Trip.Infrastructure.Data.Repositories;

public sealed class TripRepository : ITripRepository
{
    private readonly ApplicationDbContext _context;

    public TripRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TripEntity?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Trips
            .Include(t => t.Rider)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TripEntity>> GetByRiderIdAsync(
        long riderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Trips
            .Include(t => t.Rider)
            .Where(t => t.RiderId == riderId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TripEntity>> GetByDriverIdAsync(
        long driverId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Trips
            .Include(t => t.Rider)
            .Where(t => t.DriverId == driverId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TripEntity>> GetByStatusAsync(
        TripStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.Trips
            .Include(t => t.Rider)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TripEntity>> GetAllAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await _context.Trips
            .Include(t => t.Rider)
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        TripEntity trip,
        CancellationToken cancellationToken = default)
    {
        await _context.Trips.AddAsync(trip, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}

