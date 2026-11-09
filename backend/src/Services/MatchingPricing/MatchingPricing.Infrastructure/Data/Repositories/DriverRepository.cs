using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Domain.Entities;
using MatchingPricing.Domain.Enumerators;
using MatchingPricing.Infrastructure.Data.DataContext;
using Microsoft.EntityFrameworkCore;

namespace MatchingPricing.Infrastructure.Data.Repositories;

public sealed class DriverRepository : IDriverRepository
{
    private readonly ApplicationDbContext _context;

    public DriverRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Driver?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Drivers
            .Include(d => d.Assignments)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Driver?> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Drivers
            .Include(d => d.Assignments)
            .FirstOrDefaultAsync(d => d.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<bool> ExistsByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Drivers
            .AnyAsync(d => d.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Driver>> GetAllAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await _context.Drivers
            .AsNoTracking()
            .OrderBy(d => d.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Driver>> GetAvailableNearbyDriversAsync(
        decimal pickupLatitude,
        decimal pickupLongitude,
        double maxRadiusKm = 10.0,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var drivers = await _context.Drivers
            .AsNoTracking()
            .Where(d => d.Status == DriverStatus.available &&
                        d.Latitude != null &&
                        d.Longitude != null)
            .ToListAsync(cancellationToken);

        // Freshness: updated within last 60 minutes
        var freshnessThreshold = DateTimeOffset.UtcNow.AddMinutes(-60);

        var nearby = drivers
            .Where(d => d.LocationUpdatedAt == null || d.LocationUpdatedAt >= freshnessThreshold)
            .Select(d => new
            {
                Driver = d,
                Distance = CalculateDistance(
                    (double)pickupLatitude, (double)pickupLongitude,
                    (double)d.Latitude!.Value, (double)d.Longitude!.Value)
            })
            .Where(x => x.Distance <= maxRadiusKm)
            .OrderBy(x => x.Distance)
            .Take(limit)
            .Select(x => x.Driver)
            .ToList();

        return nearby;
    }

    public async Task AddAsync(
        Driver driver,
        CancellationToken cancellationToken = default)
    {
        await _context.Drivers.AddAsync(driver, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}
