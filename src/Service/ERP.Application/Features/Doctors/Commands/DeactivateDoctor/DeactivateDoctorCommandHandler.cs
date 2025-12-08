using ERP.Core;
using ERP.Core.Uow;
using ERP.Domain.Aggregates.DoctorAggregate;
using MediatR;

namespace ERP.Application.Features.Doctors.Commands.DeactivateDoctor;

public class DeactivateDoctorCommandHandler(IDoctorRepository doctorRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<DeactivateDoctorCommand, Result>
{
    public async Task<Result> Handle(DeactivateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure("Doctor not found.");
        }

        var result = doctor.Deactivate();
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }
        
        using var scope = unitOfWork.Begin();

        await doctorRepository.UpdateAsync(doctor, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
