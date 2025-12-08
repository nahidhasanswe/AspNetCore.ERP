using ERP.Core.Events;

namespace ERP.Domain.Events;

public class AppointmentBookedEvent(Guid appointmentId, Guid patientId, Guid doctorId, Guid timeSlotId)
    : IDomainEvent
{
    public Guid AppointmentId { get; } = appointmentId;
    public Guid PatientId { get; } = patientId;
    public Guid DoctorId { get; } = doctorId;
    public Guid TimeSlotId { get; } = timeSlotId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}