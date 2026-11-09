using Trip.Domain.Entities;

namespace Trip.Domain.Repositories;

public interface ITripReader
{
    Task<TripEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}
