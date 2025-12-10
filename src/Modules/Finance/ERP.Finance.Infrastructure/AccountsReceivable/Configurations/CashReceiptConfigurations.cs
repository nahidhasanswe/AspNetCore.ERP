using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsReceivable.Configurations;

public class CashReceiptConfigurations : IEntityTypeConfiguration<CashReceipt>
{
    public void Configure(EntityTypeBuilder<CashReceipt> builder)
    {
        builder.ToTable("CashReceipts");
        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.BusinessUnitId).IsRequired();
        builder.HasIndex(cr => cr.BusinessUnitId);

        builder.Property(cr => cr.CustomerId).IsRequired();
        builder.HasIndex(cr => cr.CustomerId);

        builder.Property(cr => cr.ReceiptDate).IsRequired();
        builder.HasIndex(cr => cr.ReceiptDate);

        builder.Property(cr => cr.TransactionReference).HasMaxLength(200);
        builder.Property(cr => cr.CashAccountId).IsRequired();

        builder.OwnsOne(cr => cr.TotalReceivedAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("TotalReceivedAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.OwnsOne(cr => cr.TotalAppliedAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("TotalAppliedAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("AppliedAmountCurrency").HasMaxLength(3);
        });

        builder.Property(cr => cr.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(cr => cr.Status);
    }
}