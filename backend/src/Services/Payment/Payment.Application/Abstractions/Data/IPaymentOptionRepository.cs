using Payment.Domain.Entities;

namespace Payment.Application.Abstractions.Data;

public interface IPaymentOptionRepository
{
    Task<PaymentOption?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<PaymentOption?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentOption>> GetByMethodIdAsync(
        long methodId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentOption>> GetAllActiveAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PaymentOption paymentOption,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}

