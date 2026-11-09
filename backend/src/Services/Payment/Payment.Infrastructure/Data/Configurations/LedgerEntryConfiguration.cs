using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.Entities;

namespace Payment.Infrastructure.Data.Configurations;

public sealed class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.ToTable("ledger_entries");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .ValueGeneratedOnAdd();

        builder.Property(l => l.PaymentId)
            .IsRequired();

        builder.Property(l => l.RefundId)
            .IsRequired(false);

        builder.Property(l => l.EntryType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(l => l.EntryRef)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(l => l.DebitAccount)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(l => l.CreditAccount)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(l => l.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.HasIndex(l => l.PaymentId);
        builder.HasIndex(l => l.RefundId);
        builder.HasIndex(l => l.EntryRef);

        builder.HasOne(l => l.Payment)
            .WithMany()
            .HasForeignKey(l => l.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Refund)
            .WithMany()
            .HasForeignKey(l => l.RefundId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

