using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trip.Domain.Entities;

namespace Trip.Infrastructure.Data.Configurations;

public sealed class RiderConfiguration : IEntityTypeConfiguration<Rider>
{
    public void Configure(EntityTypeBuilder<Rider> builder)
    {
        builder.ToTable("riders");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.FullName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.DateOfBirth)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(r => r.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(r => r.PhoneNumber)
            .IsUnique();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();

        builder.HasMany(r => r.Trips)
            .WithOne(t => t.Rider)
            .HasForeignKey(t => t.RiderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

