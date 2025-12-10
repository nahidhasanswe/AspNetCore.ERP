using ERP.Finance.Domain.GeneralLedger.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.GeneralLedger.Configurations;

public class LedgerLineConfigurations : IEntityTypeConfiguration<LedgerLine>
{
    public void Configure(EntityTypeBuilder<LedgerLine> builder)
    {
        builder.ToTable("LedgerLines");
        builder.HasKey(ll => ll.Id);

        builder.Property(ll => ll.BusinessUnitId).IsRequired();
        builder.HasIndex(ll => ll.BusinessUnitId);

        builder.Property(ll => ll.JournalEntryId).IsRequired();
        builder.HasIndex(ll => ll.JournalEntryId);

        builder.Property(ll => ll.AccountId).IsRequired();
        builder.HasIndex(ll => ll.AccountId);

        builder.Property(ll => ll.IsDebit).IsRequired();
        builder.Property(ll => ll.Description).HasMaxLength(500);

        builder.OwnsOne(ll => ll.Amount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.OwnsOne(ll => ll.BaseAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("BaseAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("BaseCurrency").HasMaxLength(3);
        });

        builder.Property(ll => ll.CostCenterId);
        builder.HasIndex(ll => ll.CostCenterId);
    }
}