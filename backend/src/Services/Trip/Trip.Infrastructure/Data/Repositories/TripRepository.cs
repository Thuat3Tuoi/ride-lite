using Microsoft.EntityFrameworkCore;
using Trip.Domain.Repositories;
using Trip.Infrastructure.Data.DataContext;
using Trip.Domain.Entities;

namespace Trip.Infrastructure.Data.Repositories;

public sealed class TripRepository : ITripReader, ITripWriter
{
    private readonly TripDbContext _dbContext;

    public TripRepository(TripDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<TripEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        _dbContext.Trips.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddAsync(TripEntity trip, CancellationToken cancellationToken = default) =>
        await _dbContext.Trips.AddAsync(trip, cancellationToken);

    public Task UpdateAsync(TripEntity trip, CancellationToken cancellationToken = default)
    {
        if (_dbContext.Entry(trip).State == EntityState.Detached)
        {
            _dbContext.Trips.Update(trip);
        }

        return Task.CompletedTask;
    }
}
