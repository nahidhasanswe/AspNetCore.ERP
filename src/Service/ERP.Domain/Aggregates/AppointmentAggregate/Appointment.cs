using ERP.Core;
using ERP.Core.Aggregates;
using ERP.Domain.Aggregates.ClinicAggregate;
using ERP.Domain.Aggregates.DoctorAggregate;
using ERP.Domain.Aggregates.PatientAggregate;
using ERP.Domain.Enums;
using ERP.Domain.Events;

namespace ERP.Domain.Aggregates.AppointmentAggregate;

public class Appointment : AggregateRoot
{
    public Guid TimeSlotId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid ClinicId { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime BookingDate { get; private set; }
    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public TimeSlot TimeSlot { get; private set; } = null!; // Navigation
    public Patient Patient { get; private set; } = null!; // Navigation
    public Doctor Doctor { get; private set; } = null!; // Navigation
    public Clinic Clinic { get; private set; } = null!; // Navigation

    private Appointment() { } // EF Core

    public Appointment(
        Guid timeSlotId,
        Guid patientId,
        Guid doctorId,
        Guid clinicId,
        string? notes = null)
    {
        TimeSlotId = timeSlotId;
        PatientId = patientId;
        DoctorId = doctorId;
        ClinicId = clinicId;
        Status = AppointmentStatus.Confirmed;
        BookingDate = DateTime.UtcNow;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AppointmentBookedEvent(Id, patientId, doctorId, timeSlotId));
    }

    public Result Cancel(string reason)
    {
        if (Status == AppointmentStatus.Cancelled)
            return Result.Failure("Appointment is already cancelled");

        if (Status == AppointmentStatus.Completed)
            return Result.Failure("Cannot cancel a completed appointment");

        // Check if cancellation is within 2 hours of appointment
        var appointmentTime = TimeSlot.SlotStartDateTime;
        if (DateTime.UtcNow >= appointmentTime.AddHours(-2))
            return Result.Failure("Cannot cancel appointment within 2 hours of scheduled time");

        Status = AppointmentStatus.Cancelled;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new AppointmentCancelledEvent(Id, PatientId, DoctorId, TimeSlotId, reason));
        return Result.Success();
    }

    public Result Complete()
    {
        if (Status != AppointmentStatus.Confirmed)
            return Result.Failure("Only confirmed appointments can be completed");

        Status = AppointmentStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result MarkAsNoShow()
    {
        if (Status != AppointmentStatus.Confirmed)
            return Result.Failure("Only confirmed appointments can be marked as no-show");

        Status = AppointmentStatus.NoShow;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result UpdateNotes(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}