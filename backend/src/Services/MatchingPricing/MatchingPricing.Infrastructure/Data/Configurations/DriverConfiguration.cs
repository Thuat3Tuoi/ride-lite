using MatchingPricing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MatchingPricing.Infrastructure.Data.Configurations;

public sealed class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("drivers");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .ValueGeneratedOnAdd();

        builder.Property(d => d.FullName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.DateOfBirth)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(d => d.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(d => d.PhoneNumber)
            .IsUnique();

        builder.Property(d => d.Latitude)
            .HasPrecision(10, 7)
            .IsRequired(false);

        builder.Property(d => d.Longitude)
            .HasPrecision(10, 7)
            .IsRequired(false);

        builder.Property(d => d.Location)
            .HasColumnType("geography(Point,4326)")
            .IsRequired(false);

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(d => d.LocationUpdatedAt)
            .IsRequired(false);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .IsRequired();

        // Index on status
        builder.HasIndex(d => d.Status);

        // GiST index on location in PostgreSQL (handled gracefully in non-PostgreSQL providers)
        builder.HasIndex(d => d.Location)
            .HasMethod("GIST");

        builder.HasMany(d => d.Assignments)
            .WithOne(a => a.Driver)
            .HasForeignKey(a => a.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

