using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.FixedAssetManagement.Configurations;

public class FixedAssetConfigurations : IEntityTypeConfiguration<FixedAsset>
{
    public void Configure(EntityTypeBuilder<FixedAsset> builder)
    {
        builder.ToTable("FixedAssets");
        builder.HasKey(fa => fa.Id);

        builder.Property(fa => fa.BusinessUnitId).IsRequired();
        builder.HasIndex(fa => fa.BusinessUnitId);

        builder.Property(fa => fa.TagNumber).IsRequired().HasMaxLength(100);
        builder.HasIndex(fa => fa.TagNumber).IsUnique();

        builder.Property(fa => fa.Description).IsRequired().HasMaxLength(500);
        builder.Property(fa => fa.AcquisitionDate).IsRequired();
        builder.Property(fa => fa.AssetAccountId).IsRequired();
        builder.Property(fa => fa.DepreciationExpenseAccountId).IsRequired();
        builder.Property(fa => fa.AccumulatedDepreciationAccountId).IsRequired();
        builder.Property(fa => fa.Location).HasMaxLength(200);
        builder.Property(fa => fa.TotalAccumulatedDepreciation).HasColumnType("decimal(18,2)");

        builder.OwnsOne(fa => fa.AcquisitionCost, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("AcquisitionCost").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.OwnsOne(fa => fa.Schedule, schedule =>
        {
            schedule.Property(s => s.Method).HasColumnName("DepreciationMethod").HasConversion<string>().HasMaxLength(50);
            schedule.Property(s => s.UsefulLifeYears).HasColumnName("UsefulLifeYears");
            schedule.Property(s => s.SalvageValue).HasColumnName("SalvageValue").HasColumnType("decimal(18,2)");
        });

        builder.Property(fa => fa.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(fa => fa.Status);

        builder.HasIndex(fa => fa.CostCenterId);
    }
}