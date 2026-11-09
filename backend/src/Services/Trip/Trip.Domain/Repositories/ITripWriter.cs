using Trip.Domain.Entities;

namespace Trip.Domain.Repositories;

public interface ITripWriter
{
    Task AddAsync(TripEntity trip, CancellationToken cancellationToken = default);

    Task UpdateAsync(TripEntity trip, CancellationToken cancellationToken = default);
}
