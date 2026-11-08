using Trip.Domain.Entities;

namespace Trip.Application.Abstractions;

public interface ITripReader
{
    Task<TripEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}
