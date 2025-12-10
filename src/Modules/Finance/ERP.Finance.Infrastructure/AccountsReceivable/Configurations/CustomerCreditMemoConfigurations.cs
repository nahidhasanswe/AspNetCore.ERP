using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsReceivable.Configurations;

public class CustomerCreditMemoConfigurations : IEntityTypeConfiguration<CustomerCreditMemo>
{
    public void Configure(EntityTypeBuilder<CustomerCreditMemo> builder)
    {
        builder.ToTable("CustomerCreditMemos");
        builder.HasKey(cm => cm.Id);

        builder.Property(cm => cm.BusinessUnitId).IsRequired();
        builder.HasIndex(cm => cm.BusinessUnitId);

        builder.Property(cm => cm.CustomerId).IsRequired();
        builder.HasIndex(cm => cm.CustomerId);

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