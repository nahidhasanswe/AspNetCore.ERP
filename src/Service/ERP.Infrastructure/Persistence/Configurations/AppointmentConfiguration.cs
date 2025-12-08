using ERP.Domain.Aggregates.AppointmentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.BookingDate)
            .IsRequired();

        builder.Property(a => a.Notes)
            .HasMaxLength(1000);

        builder.Property(a => a.CancellationReason)
            .HasMaxLength(500);

        builder.HasOne(a => a.TimeSlot)
            .WithOne()
            .HasForeignKey<Appointment>(a => a.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Patient)
            .WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Doctor)
            .WithMany()
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Clinic)
            .WithMany()
            .HasForeignKey(a => a.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.TimeSlotId)
            .IsUnique();

        builder.HasIndex(a => new { a.PatientId, a.Status })
            .HasDatabaseName("IX_Appointments_PatientId_Status");

        builder.HasIndex(a => new { a.DoctorId, a.Status })
            .HasDatabaseName("IX_Appointments_DoctorId_Status");

        builder.HasIndex(a => a.BookingDate)
            .HasDatabaseName("IX_Appointments_BookingDate");
    }
}