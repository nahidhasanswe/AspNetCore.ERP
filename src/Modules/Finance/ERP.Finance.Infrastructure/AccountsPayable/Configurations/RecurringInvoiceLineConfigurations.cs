using ERP.Finance.Domain.AccountsPayable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsPayable.Configurations;

public class RecurringInvoiceLineConfigurations : IEntityTypeConfiguration<RecurringInvoiceLine>
{
    public void Configure(EntityTypeBuilder<RecurringInvoiceLine> builder)
    {
        builder.ToTable("RecurringInvoiceLines");
        builder.HasKey("Id"); // Shadow property for primary key

        builder.Property(ril => ril.Description).IsRequired().HasMaxLength(500);
        builder.Property(ril => ril.ExpenseAccountId).IsRequired();

        builder.OwnsOne(ril => ril.LineAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("LineAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });
    }
}