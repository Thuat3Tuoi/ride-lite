using Microsoft.EntityFrameworkCore;
using Trip.Domain.Entities;
using TripEntity = Trip.Domain.Entities.Trip;

namespace Trip.Infrastructure.Data.DataContext;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Rider> Riders => Set<Rider>();
    public DbSet<TripEntity> Trips => Set<TripEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

