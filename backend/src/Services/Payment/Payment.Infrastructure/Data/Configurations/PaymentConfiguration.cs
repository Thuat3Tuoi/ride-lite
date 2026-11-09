using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentEntity = Payment.Domain.Entities.Payment;

namespace Payment.Infrastructure.Data.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<PaymentEntity>
{
    public void Configure(EntityTypeBuilder<PaymentEntity> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.TripId)
            .IsRequired();

        builder.Property(p => p.PaymentOptionId)
            .IsRequired();

        builder.Property(p => p.ProviderId)
            .IsRequired(false);

        builder.Property(p => p.ProviderTransactionId)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(p => p.GatewayToken)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.IdempotencyKey)
            .HasMaxLength(128)
            .IsRequired(false);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(p => p.PaidAt)
            .IsRequired(false);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.HasIndex(p => p.TripId);

        builder.HasIndex(p => p.IdempotencyKey)
            .IsUnique()
            .HasFilter("\"IdempotencyKey\" IS NOT NULL");

        builder.HasOne(p => p.Option)
            .WithMany()
            .HasForeignKey(p => p.PaymentOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Provider)
            .WithMany()
            .HasForeignKey(p => p.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Refunds)
            .WithOne(r => r.Payment)
            .HasForeignKey(r => r.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

