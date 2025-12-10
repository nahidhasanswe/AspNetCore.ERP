using ERP.Finance.Domain.GeneralLedger.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.GeneralLedger.Configurations;

public class GeneralLedgerEntryConfigurations : IEntityTypeConfiguration<GeneralLedgerEntry>
{
    public void Configure(EntityTypeBuilder<GeneralLedgerEntry> builder)
    {
        builder.ToTable("GeneralLedgerEntries");
        builder.HasKey(gle => gle.Id);

        builder.Property(gle => gle.AccountId).IsRequired();
        builder.HasIndex(gle => gle.AccountId);

        builder.Property(gle => gle.PostingDate).IsRequired();
        builder.HasIndex(gle => gle.PostingDate);

        builder.Property(gle => gle.JournalEntryId).IsRequired();
        builder.HasIndex(gle => gle.JournalEntryId);

        builder.Property(gle => gle.Debit).HasColumnType("decimal(18,2)");
        builder.Property(gle => gle.Credit).HasColumnType("decimal(18,2)");
        builder.Property(gle => gle.Currency).IsRequired().HasMaxLength(3);
        builder.Property(gle => gle.Description).HasMaxLength(500);

        builder.Property(gle => gle.CostCenterId);
        builder.HasIndex(gle => gle.CostCenterId);
    }
}