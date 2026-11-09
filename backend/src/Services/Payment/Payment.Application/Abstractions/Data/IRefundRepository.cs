using Payment.Domain.Entities;

namespace Payment.Application.Abstractions.Data;

public interface IRefundRepository
{
    Task<Refund?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Refund>> GetByPaymentIdAsync(
        long paymentId,
        CancellationToken cancellationToken = default);

    Task<Refund?> GetByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Refund refund,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}

