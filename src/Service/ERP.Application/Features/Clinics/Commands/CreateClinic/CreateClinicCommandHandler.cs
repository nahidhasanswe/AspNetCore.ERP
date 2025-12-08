using ERP.Core;
using ERP.Core.Uow;
using ERP.Domain.Aggregates.ClinicAggregate;
using ERP.Domain.ValueObjects;
using MediatR;

namespace ERP.Application.Features.Clinics.Commands.CreateClinic;

public class CreateClinicCommandHandler(IClinicRepository clinicRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<CreateClinicCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateClinicCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(
            request.Street,
            request.City,
            request.State,
            request.Country,
            request.PostalCode);

        var contactInfo = new ContactInfo(request.Phone, request.Email);

        var clinic = new Clinic(request.Name, address, contactInfo);
        
        using var scope = unitOfWork.Begin();
        
        await clinicRepository.AddAsync(clinic, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(clinic.Id);
    }
}