using MatchingPricing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchingPricing.Infrastructure.Data.DataContext;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<DriverAssignment> DriverAssignments => Set<DriverAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // When running with SQLite (e.g. In-Memory testing), override Postgres-specific spatial types
        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            modelBuilder.Entity<Driver>(b =>
            {
                b.Property(d => d.Location)
                    .HasColumnType("TEXT");
            });
        }
    }
}
