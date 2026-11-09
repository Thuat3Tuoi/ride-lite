using Payment.Domain.Entities;

namespace Payment.Application.Abstractions.Data;

public interface IPaymentMethodRepository
{
    Task<PaymentMethod?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<PaymentMethod?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentMethod>> GetAllActiveAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PaymentMethod paymentMethod,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}

