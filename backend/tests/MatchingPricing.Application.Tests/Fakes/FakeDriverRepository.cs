using MatchingPricing.Application.Abstractions.Data;
using MatchingPricing.Domain.Entities;
using MatchingPricing.Domain.Enumerators;

namespace MatchingPricing.Application.Tests.Fakes;

public sealed class FakeDriverRepository : IDriverRepository
{
    private long _nextId = 1;
    public List<Driver> Drivers { get; } = [];
    public bool Saved { get; private set; }

    public Task<Driver?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Drivers.FirstOrDefault(d => d.Id == id));
    }

    public Task<Driver?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Drivers.FirstOrDefault(d => d.PhoneNumber == phoneNumber));
    }

    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Drivers.Any(d => d.PhoneNumber == phoneNumber));
    }

    public Task<IReadOnlyList<Driver>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Driver> result = Drivers.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<Driver>> GetAvailableNearbyDriversAsync(
        decimal pickupLatitude,
        decimal pickupLongitude,
        double maxRadiusKm = 10.0,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var available = Drivers.Where(d => d.Status == DriverStatus.available && d.Latitude != null && d.Longitude != null).Take(limit).ToList();
        IReadOnlyList<Driver> result = available;
        return Task.FromResult(result);
    }

    public Task AddAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        if (driver.Id == 0)
        {
            typeof(Driver).GetProperty("Id")?.SetValue(driver, _nextId++);
        }

        Drivers.Add(driver);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Saved = true;
        return Task.CompletedTask;
    }
}

