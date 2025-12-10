using ERP.Finance.Domain.FiscalYear.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.FiscalYear.Configurations;

public class FiscalPeriodConfigurations : IEntityTypeConfiguration<FiscalPeriod>
{
    public void Configure(EntityTypeBuilder<FiscalPeriod> builder)
    {
        builder.ToTable("FiscalPeriods");
        builder.HasKey(fp => fp.Id);

        builder.Property(fp => fp.BusinessUnitId).IsRequired();
        builder.HasIndex(fp => fp.BusinessUnitId);

        builder.Property(fp => fp.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(fp => new { fp.BusinessUnitId, fp.Name }).IsUnique();

        builder.Property(fp => fp.StartDate).IsRequired();
        builder.HasIndex(fp => fp.StartDate);

        builder.Property(fp => fp.EndDate).IsRequired();
        builder.HasIndex(fp => fp.EndDate);

        builder.Property(fp => fp.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(fp => fp.Status);
    }
}