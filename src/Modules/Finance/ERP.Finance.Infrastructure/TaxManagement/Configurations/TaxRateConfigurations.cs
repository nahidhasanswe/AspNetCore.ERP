using ERP.Finance.Domain.TaxManagement.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.TaxManagement.Configurations;

public class TaxRateConfigurations : IEntityTypeConfiguration<TaxRate>
{
    public void Configure(EntityTypeBuilder<TaxRate> builder)
    {
        builder.ToTable("TaxRates");
        builder.HasKey(tr => tr.Id);

        builder.Property(tr => tr.JurisdictionId).IsRequired();
        builder.Property(tr => tr.Rate).HasColumnType("decimal(18,4)");
        builder.Property(tr => tr.EffectiveDate).IsRequired();
        builder.Property(tr => tr.IsActive).IsRequired();

        // A tax rate should be unique for a given jurisdiction and effective date.
        builder.HasIndex(tr => new { tr.JurisdictionId, tr.EffectiveDate }).IsUnique();

        builder.HasIndex(tr => tr.IsActive);
    }
}