using MatchingPricing.Domain.Entities;
using MatchingPricing.Domain.Enumerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MatchingPricing.Infrastructure.Data.Configurations;

public sealed class DriverAssignmentConfiguration : IEntityTypeConfiguration<DriverAssignment>
{
    public void Configure(EntityTypeBuilder<DriverAssignment> builder)
    {
        builder.ToTable("driver_assignments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();

        // Logical ID in trip_db, NOT a SQL foreign key.
        builder.Property(a => a.TripId)
            .IsRequired();

        // Local FK in matching_db.
        builder.Property(a => a.DriverId)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasDefaultValue(AssignmentStatus.OFFERED)
            .IsRequired();

        builder.Property(a => a.OfferedAt)
            .IsRequired();

        builder.Property(a => a.ExpiresAt)
            .IsRequired();

        builder.Property(a => a.RespondedAt)
            .IsRequired(false);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(a => new { a.TripId, a.Status });
        builder.HasIndex(a => new { a.DriverId, a.Status });
        builder.HasIndex(a => a.ExpiresAt);

        // Relationship
        builder.HasOne(a => a.Driver)
            .WithMany(d => d.Assignments)
            .HasForeignKey(a => a.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

