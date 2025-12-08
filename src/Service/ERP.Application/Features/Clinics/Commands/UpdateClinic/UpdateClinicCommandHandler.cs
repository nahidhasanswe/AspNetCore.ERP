using ERP.Core;
using ERP.Core.Uow;
using ERP.Domain.Aggregates.ClinicAggregate;
using ERP.Domain.ValueObjects;
using MediatR;

namespace ERP.Application.Features.Clinics.Commands.UpdateClinic;

public class UpdateClinicCommandHandler(IClinicRepository clinicRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateClinicCommand, Result>
{
    public async Task<Result> Handle(UpdateClinicCommand request, CancellationToken cancellationToken)
    {
        var clinic = await clinicRepository.GetByIdAsync(request.Id, cancellationToken);
        if (clinic is null)
            return Result.Failure("Clinic not found.");

        // 1. Create Value Objects from Command data
        var newAddress = new Address(request.Street, request.City, request.State, request.Country, request.PostalCode);
        var newContactInfo = new ContactInfo(request.Phone, request.Email);

        // 2. Execute Domain Logic: UpdateInfo (checks for name validity and updates AR)
        var result = clinic.UpdateInfo(request.Name, newAddress, newContactInfo);
        
        if (result.IsFailure)
            return result;
        
        using var scope = unitOfWork.Begin();

        // 3. Persist
        await clinicRepository.UpdateAsync(clinic, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}