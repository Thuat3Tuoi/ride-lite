using Payment.Domain.Entities;

namespace Payment.Application.Abstractions.Data;

public interface IPaymentProviderRepository
{
    Task<PaymentProvider?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<PaymentProvider?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentProvider>> GetAllActiveAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PaymentProvider paymentProvider,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}

