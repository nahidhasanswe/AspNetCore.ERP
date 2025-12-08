using ERP.Domain.Aggregates.DoctorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Specialization)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.LicenseNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.OwnsOne(d => d.ContactInfo, contact =>
        {
            contact.Property(ci => ci.Phone).HasColumnName("Phone").HasMaxLength(20);
            contact.Property(ci => ci.Email).HasColumnName("Email").HasMaxLength(100);
        });

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(d => d.Clinic)
            .WithMany()
            .HasForeignKey(d => d.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Schedules)
            .WithOne(s => s.Doctor)
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.LicenseNumber)
            .IsUnique();

        builder.HasIndex(d => d.ClinicId)
            .HasDatabaseName("IX_Doctors_ClinicId")
            .HasFilter("[IsActive] = 1");

        builder.HasIndex(d => d.Specialization);
    }
}