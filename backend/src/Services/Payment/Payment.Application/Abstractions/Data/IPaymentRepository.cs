using PaymentEntity = Payment.Domain.Entities.Payment;

namespace Payment.Application.Abstractions.Data;

public interface IPaymentRepository
{
    Task<PaymentEntity?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<PaymentEntity?> GetByTripIdAsync(
        long tripId,
        CancellationToken cancellationToken = default);

    Task<PaymentEntity?> GetByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PaymentEntity payment,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
