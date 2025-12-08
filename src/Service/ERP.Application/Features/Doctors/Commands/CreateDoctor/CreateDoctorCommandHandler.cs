using ERP.Core;
using ERP.Core.Uow;
using ERP.Domain.Aggregates.ClinicAggregate;
using ERP.Domain.Aggregates.DoctorAggregate;
using ERP.Domain.ValueObjects;
using MediatR;

namespace ERP.Application.Features.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandHandler(
    IDoctorRepository doctorRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<CreateDoctorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        var contactInfo = new ContactInfo(request.Phone, request.Email);

        var doctor = new Doctor(
            request.FirstName,
            request.LastName,
            request.Specialization,
            request.LicenseNumber,
            contactInfo,
            request.ClinicId);

        using var scope = unitOfWork.Begin();
        
        await doctorRepository.AddAsync(doctor, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(doctor.Id);
    }
}