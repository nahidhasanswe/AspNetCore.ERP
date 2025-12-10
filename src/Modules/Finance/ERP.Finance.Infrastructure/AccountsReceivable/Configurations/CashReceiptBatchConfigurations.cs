using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace ERP.Finance.Infrastructure.AccountsReceivable.Configurations;

public class CashReceiptBatchConfigurations : IEntityTypeConfiguration<CashReceiptBatch>
{
    public void Configure(EntityTypeBuilder<CashReceiptBatch> builder)
    {
        builder.ToTable("CashReceiptBatches");
        builder.HasKey(crb => crb.Id);

        builder.Property(crb => crb.BusinessUnitId).IsRequired();
        builder.HasIndex(crb => crb.BusinessUnitId);

        builder.Property(crb => crb.BatchDate).IsRequired();
        builder.HasIndex(crb => crb.BatchDate);

        builder.Property(crb => crb.CashAccountId).IsRequired();
        builder.Property(crb => crb.Reference).HasMaxLength(200);

        builder.OwnsOne(crb => crb.TotalBatchAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("TotalBatchAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.Property(crb => crb.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(crb => crb.Status);

        builder.Property(crb => crb.ReceiptIds)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null)
            );
    }
}