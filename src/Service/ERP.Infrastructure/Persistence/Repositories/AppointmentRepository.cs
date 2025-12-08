using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Domain.Aggregates.AppointmentAggregate;
using ERP.Domain.Enums;
using ERP.Domain.Specifications.Appointment;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence.Repositories;

public class AppointmentRepository(IDbContextProvider<BookingDbContext> dbContextProvider) : EfRepository<BookingDbContext, Appointment>(dbContextProvider), IAppointmentRepository
{
    public new virtual Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return FirstOrDefaultAsync(new GetByIdSpecification(id),  cancellationToken);
    }

    public Task<Appointment?> GetByTimeSlotIdAsync(Guid timeSlotId, CancellationToken cancellationToken = default)
    {
        return FirstOrDefaultAsync(new GetByTimeSlotIdSpecification(timeSlotId), cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await ListAsync(new  GetByPatientIdSpecification(patientId), cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId, DateTime? date = null, CancellationToken cancellationToken = default)
    {
        return await ListAsync(new GetByDoctorIdSpecification(doctorId), cancellationToken);
    }

    public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(Guid doctorId, DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken = default)
    {
       return await Context.TimeSlots
           .Include(t => t.Schedule)
           .Include(t => t.Doctor)
           .Where(t => t.DoctorId == doctorId &&
                       t.Status == SlotStatus.Available &&
                       t.SlotStartDateTime >= startDate &&
                       t.SlotStartDateTime <= endDate)
           .OrderBy(t => t.SlotStartDateTime)
           .ToListAsync(cancellationToken);
    }

    public async Task<TimeSlot?> GetTimeSlotByIdAsync(Guid timeSlotId, CancellationToken cancellationToken = default)
    {
        return await Context.TimeSlots
            .Include(t => t.Doctor)
            .ThenInclude(d => d.Clinic)
            .Include(t => t.Schedule)
            .FirstOrDefaultAsync(t => t.Id == timeSlotId, cancellationToken);
    }

    public async Task AddTimeSlotAsync(TimeSlot timeSlot, CancellationToken cancellationToken = default)
    {
        await Context.TimeSlots.AddAsync(timeSlot, cancellationToken);
    }

    public void UpdateTimeSlot(TimeSlot timeSlot)
    {
        Context.TimeSlots.Update(timeSlot);
    }
}