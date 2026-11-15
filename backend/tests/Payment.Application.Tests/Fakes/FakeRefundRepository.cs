using Payment.Application.Abstractions.Data;
using Payment.Domain.Entities;

namespace Payment.Application.Tests.Fakes;

public sealed class FakeRefundRepository : IRefundRepository
{
    private long _nextId = 1;
    public List<Refund> Refunds { get; } = [];
    public bool Saved { get; private set; }

    public Task<Refund?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Refunds.FirstOrDefault(r => r.Id == id));
    }

    public Task<IReadOnlyList<Refund>> GetByPaymentIdAsync(long paymentId, CancellationToken cancellationToken = default)
    {
        var list = Refunds.Where(r => r.PaymentId == paymentId).ToList();
        return Task.FromResult<IReadOnlyList<Refund>>(list);
    }

    public Task<Refund?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Refunds.FirstOrDefault(r => r.IdempotencyKey == idempotencyKey));
    }

    public Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Refunds.Any(r => r.IdempotencyKey == idempotencyKey));
    }

    public Task AddAsync(Refund refund, CancellationToken cancellationToken = default)
    {
        if (refund.Id == 0)
        {
            typeof(Refund).GetProperty("Id")?.SetValue(refund, _nextId++);
        }
        Refunds.Add(refund);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Saved = true;
        return Task.CompletedTask;
    }
}

