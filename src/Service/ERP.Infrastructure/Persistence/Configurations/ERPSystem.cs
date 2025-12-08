using ERP.Domain.Aggregates.AppointmentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable("TimeSlots");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.SlotStartDateTime)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Version)
            .IsRowVersion();

        builder.HasOne(t => t.Doctor)
            .WithMany()
            .HasForeignKey(t => t.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Schedule)
            .WithMany()
            .HasForeignKey(t => t.ScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.DoctorId, SlotDateTime = t.SlotStartDateTime })
            .IsUnique()
            .HasDatabaseName("IX_TimeSlots_DoctorId_SlotDateTime");

        builder.HasIndex(t => new { t.DoctorId, SlotDateTime = t.SlotStartDateTime, t.Status })
            .HasDatabaseName("IX_TimeSlots_DoctorId_DateTime_Status");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_TimeSlots_Status")
            .HasFilter("[Status] = 'Available'");
    }
}