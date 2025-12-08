using ERP.Core;
using ERP.Core.Uow;
using ERP.Domain.Aggregates.DoctorAggregate;
using MediatR;

namespace ERP.Application.Features.Doctors.Commands.RemoveSchedule;

public class RemoveScheduleCommandHandler(IDoctorRepository doctorRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<RemoveScheduleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RemoveScheduleCommand request, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdWithSchedulesAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure<Guid>("Doctor not found.");
        }

        var result = doctor.RemoveSchedule(request.ScheduleId);
        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        using var scope = unitOfWork.Begin();
        
        await doctorRepository.UpdateAsync(doctor, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(request.ScheduleId);
    }
}
