using ERP.Finance.Domain.Budgeting.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.Budgeting.Configurations;

public class BudgetApprovalRuleConfigurations : IEntityTypeConfiguration<BudgetApprovalRule>
{
    public void Configure(EntityTypeBuilder<BudgetApprovalRule> builder)
    {
        builder.ToTable("BudgetApprovalRules");
        builder.HasKey(bar => bar.Id);

        builder.Property(bar => bar.BusinessUnitId).IsRequired();
        builder.HasIndex(bar => bar.BusinessUnitId);

        builder.Property(bar => bar.RequiredApproverId).IsRequired();

        builder.OwnsOne(bar => bar.AmountThreshold, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("AmountThreshold").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });
    }
}