using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsReceivable.Configurations;

public class CustomerConfigurations : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(c => c.Name);

        builder.Property(c => c.ContactEmail).HasMaxLength(100);
        builder.Property(c => c.PaymentTerms).HasMaxLength(100);
        builder.Property(c => c.DefaultCurrency).HasMaxLength(3);
        builder.Property(c => c.ARControlAccountId).IsRequired();

        builder.OwnsOne(c => c.BillingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("BillingStreet").HasMaxLength(200);
            address.Property(a => a.City).HasColumnName("BillingCity").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("BillingState").HasMaxLength(100);
            address.Property(a => a.Country).HasColumnName("BillingCountry").HasMaxLength(100);
            address.Property(a => a.PostalCode).HasColumnName("BillingPostalCode").HasMaxLength(20);
        });

        builder.OwnsOne(c => c.ContactInfo, contact =>
        {
            contact.Property(ci => ci.Phone).HasColumnName("ContactPhone").HasMaxLength(50);
            contact.Property(ci => ci.Email).HasColumnName("ContactInfoEmail").HasMaxLength(100);
        });

        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(c => c.Status);

        builder.Property(c => c.CustomerCreditProfileId);
        builder.HasIndex(c => c.CustomerCreditProfileId).IsUnique();
    }
}