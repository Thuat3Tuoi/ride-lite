using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.Entities;

namespace Payment.Infrastructure.Data.Configurations;

public sealed class PaymentOptionConfiguration : IEntityTypeConfiguration<PaymentOption>
{
    public void Configure(EntityTypeBuilder<PaymentOption> builder)
    {
        builder.ToTable("payment_options");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.MethodId)
            .IsRequired();

        builder.Property(o => o.ProviderId)
            .IsRequired();

        builder.Property(o => o.Code)
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(o => o.Code)
            .IsUnique();

        builder.Property(o => o.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(o => o.Active)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(o => o.Method)
            .WithMany(m => m.Options)
            .HasForeignKey(o => o.MethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Provider)
            .WithMany(p => p.Options)
            .HasForeignKey(o => o.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

