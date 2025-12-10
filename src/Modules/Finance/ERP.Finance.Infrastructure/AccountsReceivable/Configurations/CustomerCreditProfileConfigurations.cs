using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsReceivable.Configurations;

public class CustomerCreditProfileConfigurations : IEntityTypeConfiguration<CustomerCreditProfile>
{
    public void Configure(EntityTypeBuilder<CustomerCreditProfile> builder)
    {
        builder.ToTable("CustomerCreditProfiles");
        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.CustomerId).IsRequired();
        builder.HasIndex(cp => cp.CustomerId).IsUnique();

        builder.Property(cp => cp.CurrentExposure).HasColumnType("decimal(18,2)");
        builder.Property(cp => cp.DefaultPaymentTerms).HasMaxLength(100);

        builder.OwnsOne(cp => cp.ApprovedLimit, amount =>
        {
            amount.Property(a => a.Amount).HasColumnName("ApprovedLimit").HasColumnType("decimal(18,2)");
            amount.Property(a => a.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.Property(cp => cp.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(cp => cp.Status);
    }
}