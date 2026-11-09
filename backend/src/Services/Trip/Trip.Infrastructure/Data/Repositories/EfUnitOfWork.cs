using Trip.Application.Abstractions.Data;
using Trip.Infrastructure.Data.DataContext;

namespace Trip.Infrastructure.Data.Repositories;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly TripDbContext _dbContext;

    public EfUnitOfWork(TripDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
