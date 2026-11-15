using Trip.Application.Abstractions.Data;
using Trip.Domain.Enumerators;
using TripEntity = Trip.Domain.Entities.Trip;

namespace Trip.Application.Tests.Fakes;

public sealed class FakeTripRepository : ITripRepository
{
    private long _nextId = 1;
    public List<TripEntity> Trips { get; } = [];
    public bool Saved { get; private set; }

    public Task<TripEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Trips.FirstOrDefault(t => t.Id == id));
    }

    public Task<IReadOnlyList<TripEntity>> GetByRiderIdAsync(long riderId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TripEntity> result = Trips.Where(t => t.RiderId == riderId).ToList();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<TripEntity>> GetByDriverIdAsync(long driverId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TripEntity> result = Trips.Where(t => t.DriverId == driverId).ToList();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<TripEntity>> GetByStatusAsync(TripStatus status, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TripEntity> result = Trips.Where(t => t.Status == status).ToList();
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<TripEntity>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TripEntity> result = Trips.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(result);
    }

    public Task AddAsync(TripEntity trip, CancellationToken cancellationToken = default)
    {
        if (trip.Id == 0)
        {
            typeof(TripEntity).GetProperty("Id")?.SetValue(trip, _nextId++);
        }

        Trips.Add(trip);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Saved = true;
        return Task.CompletedTask;
    }
}

