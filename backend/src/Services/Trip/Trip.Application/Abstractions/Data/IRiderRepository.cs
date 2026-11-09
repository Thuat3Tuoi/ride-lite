using Trip.Domain.Entities;

namespace Trip.Application.Abstractions.Data;

public interface IRiderRepository
{
    Task<Rider?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Rider?> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Rider>> GetAllAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Rider rider,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}

