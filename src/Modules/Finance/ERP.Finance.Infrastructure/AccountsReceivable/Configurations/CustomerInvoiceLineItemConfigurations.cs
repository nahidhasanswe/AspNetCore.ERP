using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsReceivable.Configurations;

public class CustomerInvoiceLineItemConfigurations : IEntityTypeConfiguration<CustomerInvoiceLineItem>
{
    public void Configure(EntityTypeBuilder<CustomerInvoiceLineItem> builder)
    {
        builder.ToTable("CustomerInvoiceLineItems");
        builder.HasKey(li => li.Id);

        builder.Property(li => li.Description).IsRequired().HasMaxLength(500);
        builder.Property(li => li.RevenueAccountId).IsRequired();

        builder.OwnsOne(li => li.LineAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("LineAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });
    }
}