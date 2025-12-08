using ERP.Core;
using ERP.Core.Entities;
using ERP.Domain.Aggregates.DoctorAggregate;
using ERP.Domain.Enums;

namespace ERP.Domain.Aggregates.AppointmentAggregate;

public class TimeSlot : Entity
{
    public Guid ScheduleId { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateTime SlotStartDateTime { get; private set; }
    public DateTime SlotEndDateTime { get; private set; }

    public DateTime UpdatedAt { get; private set; }
    public SlotStatus Status { get; private set; }
    public byte[] Version { get; private set; } = null!; // Concurrency token

    public Schedule Schedule { get; private set; } = null!; // Navigation
    public Doctor Doctor { get; private set; } = null!; // Navigation

    private TimeSlot() { } // EF Core

    public TimeSlot(Guid scheduleId, Guid doctorId, DateTime slotStartDateTime,  DateTime slotEndDateTime)
    {
        ScheduleId = scheduleId;
        DoctorId = doctorId;
        SlotStartDateTime = slotStartDateTime;
        SlotEndDateTime = slotEndDateTime;
        Status = SlotStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    public Result Book()
    {
        if (Status != SlotStatus.Available)
            return Result.Failure("Slot is not available for booking");

        if (SlotStartDateTime <= DateTime.UtcNow)
            return Result.Failure("Cannot book past time slots");
        
        if (SlotEndDateTime <= DateTime.UtcNow)
            return Result.Failure("Cannot book past time slots");
        
        if (SlotStartDateTime >= SlotEndDateTime)
            return Result.Failure("Time slots cannot equal or greater than end time");

        Status = SlotStatus.Booked;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Update(DateTime slotStartDateTime,  DateTime slotEndDateTime)
    {
        if (Status != SlotStatus.Available)
            return Result.Failure("Slot is not allow for updating");

        if (SlotStartDateTime <= DateTime.UtcNow)
            return Result.Failure("Cannot update start time to past time slots");
        
        if (SlotEndDateTime <= DateTime.UtcNow)
            return Result.Failure("Cannot update end time to past time slots");
        
        if (SlotStartDateTime >= SlotEndDateTime)
            return Result.Failure("Time slots cannot equal or greater than end time");
        
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status != SlotStatus.Booked)
            return Result.Failure("Only booked slots can be cancelled");

        Status = SlotStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result MakeAvailable()
    {
        if (Status == SlotStatus.Available)
            return Result.Failure("Slot is already available");

        Status = SlotStatus.Available;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Block()
    {
        if (Status == SlotStatus.Booked)
            return Result.Failure("Cannot block a booked slot");

        Status = SlotStatus.Blocked;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}