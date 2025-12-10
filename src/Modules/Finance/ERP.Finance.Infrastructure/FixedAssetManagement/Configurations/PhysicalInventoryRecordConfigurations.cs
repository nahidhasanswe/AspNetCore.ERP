using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.FixedAssetManagement.Configurations;

public class PhysicalInventoryRecordConfigurations : IEntityTypeConfiguration<PhysicalInventoryRecord>
{
    public void Configure(EntityTypeBuilder<PhysicalInventoryRecord> builder)
    {
        builder.ToTable("PhysicalInventoryRecords");
        builder.HasKey(pir => pir.Id);

        builder.Property(pir => pir.BusinessUnitId).IsRequired();
        builder.HasIndex(pir => pir.BusinessUnitId);

        builder.Property(pir => pir.AssetId).IsRequired();
        builder.HasIndex(pir => pir.AssetId);

        builder.Property(pir => pir.CountDate).IsRequired();
        builder.HasIndex(pir => pir.CountDate);

        builder.Property(pir => pir.CountedLocation).HasMaxLength(200);
        builder.Property(pir => pir.CountedBy).IsRequired().HasMaxLength(100);
        builder.Property(pir => pir.Notes).HasMaxLength(1000);

        builder.Property(pir => pir.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(pir => pir.Status);
    }
}