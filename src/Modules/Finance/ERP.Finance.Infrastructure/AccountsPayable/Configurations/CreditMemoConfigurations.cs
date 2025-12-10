using ERP.Finance.Domain.AccountsPayable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsPayable.Configurations;

public class CreditMemoConfigurations : IEntityTypeConfiguration<CreditMemo>
{
    public void Configure(EntityTypeBuilder<CreditMemo> builder)
    {
        builder.ToTable("CreditMemos");
        builder.HasKey(cm => cm.Id);

        builder.Property(cm => cm.BusinessUnitId).IsRequired();
        builder.HasIndex(cm => cm.BusinessUnitId);

        builder.Property(cm => cm.VendorId).IsRequired();
        builder.HasIndex(cm => cm.VendorId);

        builder.Property(cm => cm.MemoDate).IsRequired();
        builder.Property(cm => cm.Reason).HasMaxLength(500);

        builder.OwnsOne(cm => cm.OriginalAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("OriginalAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.OwnsOne(cm => cm.AvailableAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("AvailableAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("AvailableAmountCurrency").HasMaxLength(3);
        });

        builder.Property(cm => cm.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(cm => cm.Status);
    }
}