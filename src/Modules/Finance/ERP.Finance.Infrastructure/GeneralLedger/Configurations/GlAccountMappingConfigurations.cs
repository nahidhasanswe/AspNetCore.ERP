using ERP.Finance.Domain.GeneralLedger.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.GeneralLedger.Configurations;

public class GlAccountMappingConfigurations : IEntityTypeConfiguration<GlAccountMapping>
{
    public void Configure(EntityTypeBuilder<GlAccountMapping> builder)
    {
        builder.ToTable("GlAccountMappings");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.BusinessUnitId).IsRequired();
        builder.Property(m => m.MappingType).HasConversion<string>().HasMaxLength(50);
        builder.Property(m => m.Currency).IsRequired().HasMaxLength(3);
        builder.Property(m => m.GlAccountId).IsRequired();

        // Composite index to ensure uniqueness of mappings
        builder.HasIndex(m => new { m.BusinessUnitId, m.MappingType, m.Currency, m.ReferenceId }).IsUnique();
    }
}