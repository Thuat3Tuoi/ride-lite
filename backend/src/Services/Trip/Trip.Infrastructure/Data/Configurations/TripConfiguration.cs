using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripEntity = Trip.Domain.Entities.Trip;

namespace Trip.Infrastructure.Data.Configurations;

public sealed class TripConfiguration : IEntityTypeConfiguration<TripEntity>
{
    public void Configure(EntityTypeBuilder<TripEntity> builder)
    {
        builder.ToTable("trips", t =>
        {
            t.HasCheckConstraint("CK_trips_cost", "\"Cost\" >= 0");
        });

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        // Logical reference to matching_db.drivers.id; nullable until assignment
        builder.Property(t => t.DriverId)
            .IsRequired(false);

        // Local FK to trip_db.riders.id
        builder.Property(t => t.RiderId)
            .IsRequired();

        builder.Property(t => t.PickupAddress)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.PickupLatitude)
            .HasPrecision(10, 7)
            .IsRequired();

        builder.Property(t => t.PickupLongitude)
            .HasPrecision(10, 7)
            .IsRequired();

        builder.Property(t => t.DropoffAddress)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.DropoffLatitude)
            .HasPrecision(10, 7)
            .IsRequired();

        builder.Property(t => t.DropoffLongitude)
            .HasPrecision(10, 7)
            .IsRequired();

        builder.Property(t => t.Cost)
            .HasPrecision(14, 0)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        builder.Property(t => t.AcceptedAt)
            .IsRequired(false);

        builder.Property(t => t.DriverArrivedAt)
            .IsRequired(false);

        builder.Property(t => t.StartedAt)
            .IsRequired(false);

        builder.Property(t => t.CompletedAt)
            .IsRequired(false);

        builder.Property(t => t.CancelledAt)
            .IsRequired(false);

        builder.Property(t => t.CancelledBy)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired(false);

        builder.Property(t => t.CancelReason)
            .IsRequired(false);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        // Indexes
        builder.HasIndex(t => t.RiderId);
        builder.HasIndex(t => t.DriverId);
        builder.HasIndex(t => new { t.Status, t.CreatedAt });

        // Relationships
        builder.HasOne(t => t.Rider)
            .WithMany(r => r.Trips)
            .HasForeignKey(t => t.RiderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

