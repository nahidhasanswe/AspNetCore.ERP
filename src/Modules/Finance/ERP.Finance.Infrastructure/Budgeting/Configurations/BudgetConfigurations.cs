using ERP.Finance.Domain.Budgeting.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace ERP.Finance.Infrastructure.Budgeting.Configurations;

public class BudgetConfigurations : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("Budgets");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BusinessUnitId).IsRequired();
        builder.HasIndex(b => b.BusinessUnitId);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(200);
        builder.Property(b => b.FiscalPeriod).IsRequired().HasMaxLength(50);
        builder.HasIndex(b => b.FiscalPeriod);

        builder.Property(b => b.StartDate).IsRequired();
        builder.Property(b => b.EndDate).IsRequired();

        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(b => b.Status);

        builder.Property(b => b.ParentBudgetId);
        builder.HasIndex(b => b.ParentBudgetId);

        builder.HasMany(b => b.Items).WithOne().HasForeignKey("BudgetId").OnDelete(DeleteBehavior.Cascade);

        builder.Property(b => b.ApproverIds)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null)
            );
    }
}