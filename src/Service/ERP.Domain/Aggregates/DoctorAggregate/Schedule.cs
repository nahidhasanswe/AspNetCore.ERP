using ERP.Core;
using ERP.Core.Entities;
using ERP.Domain.ValueObjects;

namespace ERP.Domain.Aggregates.DoctorAggregate;

public class Schedule : Entity
{
    public Guid DoctorId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeRange TimeRange { get; private set; }
    public int SlotDurationMinutes { get; private set; }
    public bool IsRecurring { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Doctor Doctor { get; private set; } = null!; // Navigation property

    private Schedule() { } // EF Core

    public Schedule(
        Guid doctorId,
        DayOfWeek dayOfWeek,
        TimeRange timeRange,
        int slotDurationMinutes,
        DateTime effectiveFrom,
        DateTime? effectiveTo = null,
        bool isRecurring = true)
    {
        if (slotDurationMinutes < 15 || slotDurationMinutes > 120)
            throw new ArgumentException("Slot duration must be between 15 and 120 minutes", nameof(slotDurationMinutes));

        if (effectiveTo.HasValue && effectiveTo.Value <= effectiveFrom)
            throw new ArgumentException("Effective to must be after effective from");

        DoctorId = doctorId;
        DayOfWeek = dayOfWeek;
        TimeRange = timeRange ?? throw new ArgumentNullException(nameof(timeRange));
        SlotDurationMinutes = slotDurationMinutes;
        IsRecurring = isRecurring;
        EffectiveFrom = effectiveFrom.Date;
        EffectiveTo = effectiveTo?.Date;
        IsActive = true;
    }

    public bool Overlaps(Schedule other)
    {
        if (DayOfWeek != other.DayOfWeek)
            return false;

        return TimeRange.Overlaps(other.TimeRange);
    }

    public bool IsEffectiveOn(DateTime date)
    {
        if (!IsActive)
            return false;

        if (date.Date < EffectiveFrom.Date)
            return false;

        if (EffectiveTo.HasValue && date.Date > EffectiveTo.Value.Date)
            return false;

        if (!IsRecurring && date.Date != EffectiveFrom.Date)
            return false;

        return date.DayOfWeek == DayOfWeek;
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Failure("Schedule is already deactivated");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}