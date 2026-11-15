using Payment.Application.Abstractions.Data;
using Payment.Domain.Entities;
using PaymentEntity = Payment.Domain.Entities.Payment;

namespace Payment.Application.Tests.Fakes;

public sealed class FakePaymentRepository : IPaymentRepository
{
    private long _nextId = 1;
    public List<PaymentEntity> Payments { get; } = [];
    public bool Saved { get; private set; }

    public Task<PaymentEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Payments.FirstOrDefault(x => x.Id == id));
    }

    public Task<PaymentEntity?> GetByTripIdAsync(long tripId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Payments.FirstOrDefault(x => x.TripId == tripId));
    }

    public Task<PaymentEntity?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Payments.FirstOrDefault(x => x.IdempotencyKey == idempotencyKey));
    }

    public Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Payments.Any(x => x.IdempotencyKey == idempotencyKey));
    }

    public Task AddAsync(PaymentEntity payment, CancellationToken cancellationToken = default)
    {
        if (payment.Id == 0)
        {
            typeof(PaymentEntity).GetProperty("Id")?.SetValue(payment, _nextId++);
        }

        Payments.Add(payment);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Saved = true;
        return Task.CompletedTask;
    }
}

