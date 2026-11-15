using Trip.Application.Abstractions.Data;
using Trip.Domain.Entities;

namespace Trip.Application.Tests.Fakes;

public sealed class FakeRiderRepository : IRiderRepository
{
    private long _nextId = 1;
    public List<Rider> Riders { get; } = [];
    public bool Saved { get; private set; }

    public Task<Rider?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Riders.FirstOrDefault(r => r.Id == id));
    }

    public Task<Rider?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Riders.FirstOrDefault(r => r.PhoneNumber == phoneNumber));
    }

    public Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Riders.Any(r => r.PhoneNumber == phoneNumber));
    }

    public Task<IReadOnlyList<Rider>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Rider> result = Riders.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(result);
    }

    public Task AddAsync(Rider rider, CancellationToken cancellationToken = default)
    {
        if (rider.Id == 0)
        {
            typeof(Rider).GetProperty("Id")?.SetValue(rider, _nextId++);
        }

        Riders.Add(rider);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Saved = true;
        return Task.CompletedTask;
    }
}

