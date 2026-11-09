using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.Entities;

namespace Payment.Infrastructure.Data.Configurations;

public sealed class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("payment_methods");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();

        builder.Property(m => m.Code)
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(m => m.Code)
            .IsUnique();

        builder.Property(m => m.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(m => m.Active)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasMany(m => m.Options)
            .WithOne(o => o.Method)
            .HasForeignKey(o => o.MethodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

