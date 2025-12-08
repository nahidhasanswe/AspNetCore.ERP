using ERP.Domain.Aggregates.DoctorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.ToTable("Schedules");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.DayOfWeek)
            .IsRequired()
            .HasConversion<int>();

        builder.OwnsOne(s => s.TimeRange, timeRange =>
        {
            timeRange.Property(tr => tr.StartTime).HasColumnName("StartTime");
            timeRange.Property(tr => tr.EndTime).HasColumnName("EndTime");
        });

        builder.Property(s => s.SlotDurationMinutes)
            .IsRequired();

        builder.Property(s => s.IsRecurring)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.EffectiveFrom)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(s => s.Doctor)
            .WithMany(d => d.Schedules)
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.DoctorId, s.IsActive })
            .HasDatabaseName("IX_Schedules_DoctorId_IsActive");

        builder.HasIndex(s => new { s.EffectiveFrom, s.EffectiveTo })
            .HasDatabaseName("IX_Schedules_DateRange");
    }
}