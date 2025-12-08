using ERP.Core;
using ERP.Core.Uow;
using ERP.Domain.Aggregates.DoctorAggregate;
using ERP.Domain.ValueObjects;
using MediatR;

namespace ERP.Application.Features.Doctors.Commands.CreateSchedule;

public class CreateScheduleCommandHandler(IDoctorRepository doctorRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<CreateScheduleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        // Fetch Doctor with its schedules to enforce Aggregate Root invariants (overlap checks)
        var doctor = await doctorRepository.GetByIdWithSchedulesAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
            return Result.Failure<Guid>("Doctor not found.");
        
        // 1. Create Domain Objects
        var startTimeSpan = TimeSpan.Parse(request.StartTime);
        var endTimeSpan = TimeSpan.Parse(request.EndTime);
        var timeRange = new TimeRange(startTimeSpan, endTimeSpan);
        
        var schedule = new Schedule(
            request.DoctorId,
            (DayOfWeek)request.DayOfWeek,
            timeRange,
            request.SlotDurationMinutes,
            request.EffectiveFrom,
            request.EffectiveTo,
            request.IsRecurring);

        // 2. Execute Domain Logic: Add Schedule (Overlap check enforced within the Doctor Aggregate)
        var result = doctor.AddSchedule(schedule);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);
        
        using var scope = unitOfWork.Begin();

        // 3. Persist the updated Doctor Aggregate (which includes the new Schedule entity)
        await doctorRepository.UpdateAsync(doctor, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        // TODO: In a production system, a domain event listener or a dedicated command 
        // would now call the ISlotGenerationService to pre-populate TimeSlots based on this new schedule.

        return Result.Success(schedule.Id);
    }
}