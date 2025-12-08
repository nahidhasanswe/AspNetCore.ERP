using ERP.Domain.Aggregates.PatientAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        builder.Property(p => p.Gender)
            .HasMaxLength(10);

        builder.OwnsOne(p => p.ContactInfo, contact =>
        {
            contact.Property(ci => ci.Phone).HasColumnName("Phone").HasMaxLength(20);
            contact.Property(ci => ci.Email).HasColumnName("Email").HasMaxLength(100);
        });

        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("AddressStreet").HasMaxLength(500);
            address.Property(a => a.City).HasColumnName("AddressCity").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("AddressState").HasMaxLength(100);
            address.Property(a => a.Country).HasColumnName("AddressCountry").HasMaxLength(100);
            address.Property(a => a.PostalCode).HasColumnName("AddressPostalCode").HasMaxLength(20);
        });

        builder.Property(p => p.EmergencyContact)
            .HasMaxLength(20);

        // builder.HasIndex(p => p.ContactInfo.Email)
        //     .IsUnique()
        //     .HasDatabaseName("IX_Patients_Email");
        //
        // builder.HasIndex(p => p.ContactInfo.Phone)
        //     .IsUnique()
        //     .HasDatabaseName("IX_Patients_Phone");
    }
}