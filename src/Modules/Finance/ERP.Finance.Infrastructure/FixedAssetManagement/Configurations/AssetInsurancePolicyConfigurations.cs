using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.FixedAssetManagement.Configurations;

public class AssetInsurancePolicyConfigurations : IEntityTypeConfiguration<AssetInsurancePolicy>
{
    public void Configure(EntityTypeBuilder<AssetInsurancePolicy> builder)
    {
        builder.ToTable("AssetInsurancePolicies");
        builder.HasKey(aip => aip.Id);

        builder.Property(aip => aip.BusinessUnitId).IsRequired();
        builder.HasIndex(aip => aip.BusinessUnitId);

        builder.Property(aip => aip.AssetId).IsRequired();
        builder.HasIndex(aip => aip.AssetId);

        builder.Property(aip => aip.PolicyNumber).IsRequired().HasMaxLength(100);
        builder.Property(aip => aip.Insurer).HasMaxLength(200);
        builder.Property(aip => aip.StartDate).IsRequired();
        builder.Property(aip => aip.EndDate).IsRequired();

        builder.OwnsOne(aip => aip.CoverageAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("CoverageAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.OwnsOne(aip => aip.PremiumAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("PremiumAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("PremiumCurrency").HasMaxLength(3);
        });

        builder.Property(aip => aip.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(aip => aip.Status);
    }
}