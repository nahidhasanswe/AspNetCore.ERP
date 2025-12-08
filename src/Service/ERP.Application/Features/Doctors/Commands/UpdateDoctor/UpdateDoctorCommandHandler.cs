using ERP.Core;
using ERP.Core.Uow;
using ERP.Domain.Aggregates.DoctorAggregate;
using ERP.Domain.ValueObjects;
using MediatR;

namespace ERP.Application.Features.Doctors.Commands.UpdateDoctor;

public class UpdateDoctorCommandHandler(
    IDoctorRepository doctorRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateDoctorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(request.Id, cancellationToken);

        if (doctor is null)
        {
            return Result.Failure<Guid>("Doctor not found");
        }

        var contactInfo = new ContactInfo(request.Phone, request.Email);

        var result = doctor.Update(
            request.FirstName,
            request.LastName,
            request.Specialization,
            request.LicenseNumber,
            contactInfo,
            request.ClinicId);

        if (result.IsFailure)
            return Result.Failure<Guid>("Doctor information cannot be updated");

        using var scope = unitOfWork.Begin();
        
        await doctorRepository.UpdateAsync(doctor, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(doctor.Id);
    }
}
