using ERP.Finance.Domain.AccountsPayable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsPayable.Configurations;

public class DebitMemoConfigurations : IEntityTypeConfiguration<DebitMemo>
{
    public void Configure(EntityTypeBuilder<DebitMemo> builder)
    {
        builder.ToTable("DebitMemos");
        builder.HasKey(dm => dm.Id);

        builder.Property(dm => dm.BusinessUnitId).IsRequired();
        builder.HasIndex(dm => dm.BusinessUnitId);

        builder.Property(dm => dm.VendorId).IsRequired();
        builder.HasIndex(dm => dm.VendorId);

        builder.Property(dm => dm.MemoDate).IsRequired();
        builder.Property(dm => dm.Reason).HasMaxLength(500);
        builder.Property(dm => dm.APControlAccountId).IsRequired();

        builder.OwnsOne(dm => dm.Amount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.Property(dm => dm.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(dm => dm.Status);
    }
}