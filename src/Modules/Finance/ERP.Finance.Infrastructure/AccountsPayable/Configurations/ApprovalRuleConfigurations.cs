using ERP.Finance.Domain.AccountsPayable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsPayable.Configurations;

public class ApprovalRuleConfigurations : IEntityTypeConfiguration<ApprovalRule>
{
    public void Configure(EntityTypeBuilder<ApprovalRule> builder)
    {
        builder.ToTable("ApprovalRules");
        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.BusinessUnitId).IsRequired();
        builder.Property(ar => ar.RequiredApproverId).IsRequired();

        builder.OwnsOne(ar => ar.AmountThreshold, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("AmountThreshold").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });
    }
}