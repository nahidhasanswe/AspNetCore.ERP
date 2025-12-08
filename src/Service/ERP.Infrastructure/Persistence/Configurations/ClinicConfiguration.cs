using ERP.Domain.Aggregates.ClinicAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        builder.ToTable("Clinics");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("Street").HasMaxLength(500);
            address.Property(a => a.City).HasColumnName("City").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("State").HasMaxLength(100);
            address.Property(a => a.Country).HasColumnName("Country").HasMaxLength(100);
            address.Property(a => a.PostalCode).HasColumnName("PostalCode").HasMaxLength(20);
        });

        builder.OwnsOne(c => c.ContactInfo, contact =>
        {
            contact.Property(ci => ci.Phone).HasColumnName("Phone").HasMaxLength(20);
            contact.Property(ci => ci.Email).HasColumnName("Email").HasMaxLength(100);
        });

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // builder.HasIndex(c => new { c.Name, c.IsActive })
        //     .HasDatabaseName("IX_Clinics_Name_City");

        // builder.HasIndex(c => c.Address.City)
        //     .HasDatabaseName("IX_Clinics_City")
        //     .HasFilter("[IsActive] = 1");
    }
}