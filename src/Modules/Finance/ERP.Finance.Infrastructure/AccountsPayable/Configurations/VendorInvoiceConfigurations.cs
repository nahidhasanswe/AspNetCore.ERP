using ERP.Finance.Domain.AccountsPayable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace ERP.Finance.Infrastructure.AccountsPayable.Configurations;

public class VendorInvoiceConfigurations : IEntityTypeConfiguration<VendorInvoice>
{
    public void Configure(EntityTypeBuilder<VendorInvoice> builder)
    {
        builder.ToTable("VendorInvoices");
        builder.HasKey(vi => vi.Id);

        builder.Property(vi => vi.BusinessUnitId).IsRequired();
        builder.HasIndex(vi => vi.BusinessUnitId);

        builder.Property(vi => vi.VendorId).IsRequired();
        builder.HasIndex(vi => vi.VendorId);

        builder.Property(vi => vi.InvoiceNumber).IsRequired().HasMaxLength(100);
        builder.Property(vi => vi.InvoiceDate).IsRequired();
        builder.HasIndex(vi => vi.InvoiceDate);

        builder.Property(vi => vi.DueDate).IsRequired();
        builder.HasIndex(vi => vi.DueDate);

        builder.Property(vi => vi.APControlAccountId).IsRequired();
        builder.Property(vi => vi.TotalPaymentsRecorded).HasColumnType("decimal(18,2)");
        builder.Property(vi => vi.TotalCreditsApplied).HasColumnType("decimal(18,2)");

        builder.OwnsOne(vi => vi.TotalAmount, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.Property(vi => vi.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(vi => vi.Status);

        builder.Property(vi => vi.MatchingStatus).HasConversion<string>().HasMaxLength(50);

        builder.HasMany(vi => vi.LineItems).WithOne().HasForeignKey("VendorInvoiceId").OnDelete(DeleteBehavior.Cascade);

        // Configure the private collection of approver IDs
        builder.Property(vi => vi.ApproverIds)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null)
            );
    }
}