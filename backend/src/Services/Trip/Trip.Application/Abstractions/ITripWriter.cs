using Trip.Domain.Entities;

namespace Trip.Application.Abstractions;

public interface ITripWriter
{
    Task AddAsync(TripEntity trip, CancellationToken cancellationToken = default);

    Task UpdateAsync(TripEntity trip, CancellationToken cancellationToken = default);
}
