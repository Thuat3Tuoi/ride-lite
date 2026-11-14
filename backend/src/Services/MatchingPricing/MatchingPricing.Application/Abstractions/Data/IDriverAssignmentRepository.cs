using MatchingPricing.Domain.Entities;

namespace MatchingPricing.Application.Abstractions.Data;

public interface IDriverAssignmentRepository
{
    Task<DriverAssignment?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DriverAssignment>> GetByTripIdAsync(
        long tripId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DriverAssignment>> GetByDriverIdAsync(
        long driverId,
        CancellationToken cancellationToken = default);

    Task<DriverAssignment?> GetAcceptedAssignmentForTripAsync(
        long tripId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        DriverAssignment assignment,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}

