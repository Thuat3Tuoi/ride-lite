using Trip.Domain.Enumerators;
using TripEntity = Trip.Domain.Entities.Trip;

namespace Trip.Application.Abstractions.Data;

public interface ITripRepository
{
    Task<TripEntity?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TripEntity>> GetByRiderIdAsync(
        long riderId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TripEntity>> GetByDriverIdAsync(
        long driverId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TripEntity>> GetByStatusAsync(
        TripStatus status,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TripEntity>> GetAllAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TripEntity trip,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}

