using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.FixedAssetManagement.Configurations;

public class AssetMaintenanceRecordConfigurations : IEntityTypeConfiguration<AssetMaintenanceRecord>
{
    public void Configure(EntityTypeBuilder<AssetMaintenanceRecord> builder)
    {
        builder.ToTable("AssetMaintenanceRecords");
        builder.HasKey(amr => amr.Id);

        builder.Property(amr => amr.BusinessUnitId).IsRequired();
        builder.HasIndex(amr => amr.BusinessUnitId);

        builder.Property(amr => amr.AssetId).IsRequired();
        builder.HasIndex(amr => amr.AssetId);

        builder.Property(amr => amr.ScheduledDate).IsRequired();
        builder.HasIndex(amr => amr.ScheduledDate);

        builder.Property(amr => amr.CompletionDate);
        builder.Property(amr => amr.Description).IsRequired().HasMaxLength(500);
        builder.Property(amr => amr.PerformedBy).HasMaxLength(100);

        builder.OwnsOne(amr => amr.Cost, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("Cost").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.Property(amr => amr.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(amr => amr.Status);
    }
}