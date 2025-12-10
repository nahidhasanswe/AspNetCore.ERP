using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsReceivable.Configurations;

public class CustomerInvoiceConfigurations : IEntityTypeConfiguration<CustomerInvoice>
{
    public void Configure(EntityTypeBuilder<CustomerInvoice> builder)
    {
        builder.ToTable("CustomerInvoices");
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.BusinessUnitId).IsRequired();
        builder.HasIndex(ci => ci.BusinessUnitId);

        builder.Property(ci => ci.CustomerId).IsRequired();
        builder.HasIndex(ci => ci.CustomerId);

        builder.Property(ci => ci.InvoiceNumber).IsRequired().HasMaxLength(100);
        builder.Property(ci => ci.IssueDate).IsRequired();
        builder.HasIndex(ci => ci.IssueDate);

        builder.Property(ci => ci.DueDate).IsRequired();
        builder.HasIndex(ci => ci.DueDate);

        builder.Property(ci => ci.ARControlAccountId).IsRequired();
        builder.Property(ci => ci.TotalPaymentsReceived).HasColumnType("decimal(18,2)");
        builder.Property(ci => ci.TotalAmountWrittenOff).HasColumnType("decimal(18,2)");
        builder.Property(ci => ci.TotalCreditsApplied).HasColumnType("decimal(18,2)");

        builder.OwnsOne(ci => ci.TotalAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.Property(ci => ci.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(ci => ci.Status);

        builder.HasMany(ci => ci.LineItems).WithOne().HasForeignKey("CustomerInvoiceId").OnDelete(DeleteBehavior.Cascade);
    }
}