using ERP.Finance.Domain.TaxManagement.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.TaxManagement.Configurations;

public class TaxJurisdictionConfigurations : IEntityTypeConfiguration<TaxJurisdiction>
{
    public void Configure(EntityTypeBuilder<TaxJurisdiction> builder)
    {
        builder.ToTable("TaxJurisdictions");
        builder.HasKey(tj => tj.Id);

        builder.Property(tj => tj.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(tj => tj.Name);

        builder.Property(tj => tj.RegionCode).IsRequired().HasMaxLength(50);
        builder.HasIndex(tj => tj.RegionCode).IsUnique();

        builder.Property(tj => tj.IsActive).IsRequired();
        builder.HasIndex(tj => tj.IsActive);
    }
}