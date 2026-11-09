using MatchingPricing.Domain.Entities;

namespace MatchingPricing.Application.Abstractions.Data;

public interface IDriverRepository
{
    Task<Driver?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Driver?> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Driver>> GetAllAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Driver>> GetAvailableNearbyDriversAsync(
        decimal pickupLatitude,
        decimal pickupLongitude,
        double maxRadiusKm = 10.0,
        int limit = 10,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Driver driver,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}

