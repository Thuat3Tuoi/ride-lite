using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Trip.Domain.Entities;

namespace Trip.Infrastructure.Data.DataContext.Configurations;

public sealed class TripEntityConfiguration : IEntityTypeConfiguration<TripEntity>
{
    private const string StatusConstraint =
        "status IN ('Requested', 'Quoted', 'AwaitingDriver', 'EnRoute', 'DriverArrived', 'PickedUp', 'DroppedOff', 'PaymentPending', 'Paid', 'Cancelled', 'Failed')";

    private const string CancelledByConstraint =
        "cancelled_by IS NULL OR cancelled_by IN ('Rider', 'Driver', 'System')";

    public void Configure(EntityTypeBuilder<TripEntity> builder)
    {
        builder.ToTable("trips", table =>
        {
            table.HasCheckConstraint("ck_trips_status", StatusConstraint);
            table.HasCheckConstraint("ck_trips_cancelled_by", CancelledByConstraint);
        });

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn();

        builder.Property(t => t.RiderId)
            .HasColumnName("rider_id")
            .IsRequired();

        builder.Property(t => t.DriverId)
            .HasColumnName("driver_id");

        builder.Property(t => t.PickupAddress)
            .HasColumnName("pickup_address")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.PickupLatitude)
            .HasColumnName("pickup_latitude")
            .HasPrecision(10, 7);

        builder.Property(t => t.PickupLongitude)
            .HasColumnName("pickup_longitude")
            .HasPrecision(10, 7);

        builder.Property(t => t.DropoffAddress)
            .HasColumnName("dropoff_address")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.DropoffLatitude)
            .HasColumnName("dropoff_latitude")
            .HasPrecision(10, 7);

        builder.Property(t => t.DropoffLongitude)
            .HasColumnName("dropoff_longitude")
            .HasPrecision(10, 7);

        builder.Property(t => t.Cost)
            .HasColumnName("cost")
            .HasPrecision(14, 0);

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(t => t.AcceptedAt)
            .HasColumnName("accepted_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(t => t.DriverArrivedAt)
            .HasColumnName("driver_arrived_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(t => t.StartedAt)
            .HasColumnName("started_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(t => t.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(t => t.CancelledAt)
            .HasColumnName("cancelled_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(t => t.CancelledBy)
            .HasColumnName("cancelled_by")
            .HasConversion<string>()
            .HasMaxLength(16);

        builder.Property(t => t.CancelReason)
            .HasColumnName("cancel_reason")
            .HasColumnType("text");

        builder.Property(t => t.DriverRejectCount)
            .HasColumnName("driver_reject_count")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property<uint>("xmin")
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.Ignore(t => t.PickupLocation);
        builder.Ignore(t => t.DropoffLocation);
        builder.Ignore(t => t.DomainEvents);
    }
}
