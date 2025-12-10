using ERP.Finance.Domain.AccountsPayable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsPayable.Configurations;

public class RecurringInvoiceConfigurations : IEntityTypeConfiguration<RecurringInvoice>
{
    public void Configure(EntityTypeBuilder<RecurringInvoice> builder)
    {
        builder.ToTable("RecurringInvoices");
        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.BusinessUnitId).IsRequired();
        builder.HasIndex(ri => ri.BusinessUnitId);

        builder.Property(ri => ri.VendorId).IsRequired();
        builder.HasIndex(ri => ri.VendorId);

        builder.Property(ri => ri.Interval).HasConversion<string>().HasMaxLength(50);
        builder.Property(ri => ri.StartDate).IsRequired();
        builder.Property(ri => ri.EndDate);

        builder.Property(ri => ri.NextOccurrenceDate).IsRequired();
        builder.HasIndex(ri => ri.NextOccurrenceDate);

        builder.Property(ri => ri.IsActive).IsRequired();
        builder.HasIndex(ri => ri.IsActive);

        builder.HasMany(ri => ri.Lines).WithOne().HasForeignKey("RecurringInvoiceId").OnDelete(DeleteBehavior.Cascade);
    }
}