using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.Encumbrance.Configurations;

public class EncumbranceConfigurations : IEntityTypeConfiguration<Domain.Encumbrance.Aggregates.Encumbrance>
{
    public void Configure(EntityTypeBuilder<Domain.Encumbrance.Aggregates.Encumbrance> builder)
    {
        builder.ToTable("Encumbrances");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.BusinessUnitId).IsRequired();
        builder.HasIndex(e => e.BusinessUnitId);

        builder.Property(e => e.SourceTransactionId).IsRequired();
        builder.HasIndex(e => e.SourceTransactionId);

        builder.Property(e => e.GlAccountId).IsRequired();
        builder.HasIndex(e => e.GlAccountId);

        builder.Property(e => e.CostCenterId);
        builder.HasIndex(e => e.CostCenterId);

        builder.OwnsOne(e => e.Amount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(e => e.Type);

        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(e => e.Status);
    }
}