using ERP.Finance.Domain.Budgeting.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.Budgeting.Configurations;

public class BudgetItemConfigurations : IEntityTypeConfiguration<BudgetItem>
{
    public void Configure(EntityTypeBuilder<BudgetItem> builder)
    {
        builder.ToTable("BudgetItems");
        builder.HasKey(bi => bi.Id);

        builder.Property(bi => bi.AccountId).IsRequired();
        builder.HasIndex(bi => bi.AccountId);

        builder.Property(bi => bi.Period).IsRequired().HasMaxLength(50);
        builder.HasIndex(bi => bi.Period);

        builder.Property(bi => bi.CostCenterId);
        builder.HasIndex(bi => bi.CostCenterId);

        builder.OwnsOne(bi => bi.BudgetedAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("BudgetedAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.OwnsOne(bi => bi.CommittedAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("CommittedAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("CommittedAmountCurrency").HasMaxLength(3);
        });
    }
}