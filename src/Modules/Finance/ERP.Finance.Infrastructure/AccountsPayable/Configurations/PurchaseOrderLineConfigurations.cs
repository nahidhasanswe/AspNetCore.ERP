using ERP.Finance.Domain.AccountsPayable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsPayable.Configurations;

public class PurchaseOrderLineConfigurations : IEntityTypeConfiguration<PurchaseOrderLine>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
    {
        builder.ToTable("PurchaseOrderLines");
        builder.HasKey(pol => pol.Id);

        builder.Property(pol => pol.ProductId).IsRequired();
        builder.Property(pol => pol.Description).IsRequired().HasMaxLength(500);
        builder.Property(pol => pol.Quantity).HasColumnType("decimal(18,2)");

        builder.OwnsOne(pol => pol.UnitPrice, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("UnitPrice").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });
    }
}