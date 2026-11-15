using Payment.Application.Abstractions.Data;
using Payment.Domain.Entities;

namespace Payment.Application.Tests.Fakes;

public sealed class FakePaymentMethodRepository : IPaymentMethodRepository
{
    private long _nextId = 1;
    public List<PaymentMethod> Methods { get; } = [];
    public bool Saved { get; private set; }

    public Task<PaymentMethod?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Methods.FirstOrDefault(m => m.Id == id));
    }

    public Task<PaymentMethod?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Methods.FirstOrDefault(m => m.Code == code));
    }

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Methods.Any(m => m.Code == code));
    }

    public Task<IReadOnlyList<PaymentMethod>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        var list = Methods.Where(m => m.Active).ToList();
        return Task.FromResult<IReadOnlyList<PaymentMethod>>(list);
    }

    public Task AddAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        if (paymentMethod.Id == 0)
        {
            typeof(PaymentMethod).GetProperty("Id")?.SetValue(paymentMethod, _nextId++);
        }
        Methods.Add(paymentMethod);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Saved = true;
        return Task.CompletedTask;
    }
}

