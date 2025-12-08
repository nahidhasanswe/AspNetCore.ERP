using ERP.Core.Events;

namespace ERP.Domain.Events;

public class AppointmentCancelledEvent(
    Guid appointmentId,
    Guid patientId,
    Guid doctorId,
    Guid timeSlotId,
    string reason)
    : IDomainEvent
{
    public Guid AppointmentId { get; } = appointmentId;
    public Guid PatientId { get; } = patientId;
    public Guid DoctorId { get; } = doctorId;
    public Guid TimeSlotId { get; } = timeSlotId;
    public string Reason { get; } = reason;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}