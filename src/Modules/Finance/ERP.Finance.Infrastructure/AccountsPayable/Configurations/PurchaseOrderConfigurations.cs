using ERP.Finance.Domain.AccountsPayable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsPayable.Configurations;

public class PurchaseOrderConfigurations : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");
        builder.HasKey(po => po.Id);

        builder.Property(po => po.BusinessUnitId).IsRequired();
        builder.HasIndex(po => po.BusinessUnitId);

        builder.Property(po => po.VendorId).IsRequired();
        builder.HasIndex(po => po.VendorId);

        builder.Property(po => po.OrderDate).IsRequired();

        builder.Property(po => po.Status).HasConversion<string>().HasMaxLength(50);

        builder.HasMany(po => po.Lines).WithOne().HasForeignKey("PurchaseOrderId").OnDelete(DeleteBehavior.Cascade);
    }
}