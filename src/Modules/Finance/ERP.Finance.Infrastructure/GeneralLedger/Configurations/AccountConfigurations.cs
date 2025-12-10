using ERP.Finance.Domain.GeneralLedger.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.GeneralLedger.Configurations;

public class AccountConfigurations : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.BusinessUnitId).IsRequired();
        builder.HasIndex(a => a.BusinessUnitId);

        builder.Property(a => a.AccountCode).IsRequired().HasMaxLength(50);
        builder.HasIndex(a => new { a.BusinessUnitId, a.AccountCode }).IsUnique();

        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(a => a.Type);

        builder.Property(a => a.ParentId);
        builder.HasIndex(a => a.ParentId);

        builder.Property(a => a.IsSummary).IsRequired();
        builder.Property(a => a.IsActive).IsRequired();
        builder.HasIndex(a => a.IsActive);
    }
}