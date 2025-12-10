using ERP.Finance.Domain.FixedAssetManagement.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.FixedAssetManagement.Configurations;

public class LeasedAssetConfigurations : IEntityTypeConfiguration<LeasedAsset>
{
    public void Configure(EntityTypeBuilder<LeasedAsset> builder)
    {
        builder.ToTable("LeasedAssets");
        builder.HasKey(la => la.Id);

        builder.Property(la => la.BusinessUnitId).IsRequired();
        builder.HasIndex(la => la.BusinessUnitId);

        builder.Property(la => la.AssetId).IsRequired();
        builder.HasIndex(la => la.AssetId);

        builder.Property(la => la.LeaseAgreementNumber).IsRequired().HasMaxLength(100);
        builder.Property(la => la.Lessor).HasMaxLength(200);
        builder.Property(la => la.StartDate).IsRequired();
        builder.Property(la => la.EndDate).IsRequired();

        builder.OwnsOne(la => la.MonthlyPayment, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("MonthlyPayment").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.Property(la => la.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(la => la.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(la => la.Status);
    }
}