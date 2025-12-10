using ERP.Finance.Domain.AccountsPayable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsPayable.Configurations;

public class VendorConfigurations : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.ToTable("Vendors");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(v => v.Name);

        builder.Property(v => v.TaxId).HasMaxLength(50);
        builder.Property(v => v.DefaultCurrency).HasMaxLength(3);

        builder.OwnsOne(v => v.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("Street").HasMaxLength(200);
            address.Property(a => a.City).HasColumnName("City").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("State").HasMaxLength(100);
            address.Property(a => a.Country).HasColumnName("Country").HasMaxLength(100);
            address.Property(a => a.PostalCode).HasColumnName("PostalCode").HasMaxLength(20);
        });

        builder.OwnsOne(v => v.ContactInfo, contact =>
        {
            contact.Property(c => c.Phone).HasColumnName("Phone").HasMaxLength(50);
            contact.Property(c => c.Email).HasColumnName("Email").HasMaxLength(100);
        });
    }
}