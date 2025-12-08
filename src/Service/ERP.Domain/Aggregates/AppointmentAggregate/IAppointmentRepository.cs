using ERP.Core.Repository;

namespace ERP.Domain.Aggregates.AppointmentAggregate;

public interface IAppointmentRepository: IRepository<Appointment>
{
    Task<Appointment?> GetByTimeSlotIdAsync(Guid timeSlotId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId, DateTime? date = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(Guid doctorId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<TimeSlot?> GetTimeSlotByIdAsync(Guid timeSlotId, CancellationToken cancellationToken = default);
    Task AddTimeSlotAsync(TimeSlot timeSlot, CancellationToken cancellationToken = default);
    void UpdateTimeSlot(TimeSlot timeSlot);
}