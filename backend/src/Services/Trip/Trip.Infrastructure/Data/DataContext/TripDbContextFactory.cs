using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Trip.Infrastructure.Data.DataContext;

public sealed class TripDbContextFactory : IDesignTimeDbContextFactory<TripDbContext>
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;Database=ridelite_trip;Username=postgres;Password=postgres";

    public TripDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TripDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new TripDbContext(options);
    }
}
