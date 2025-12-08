using ERP.Core;
using ERP.Core.Uow;
using ERP.Domain.Aggregates.PatientAggregate;
using ERP.Domain.ValueObjects;
using MediatR;

namespace ERP.Application.Features.Patient.Commands.UpdatePatient;

public class UpdatePatientCommandHandler(IPatientRepository patientRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdatePatientCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var existingPatient = await patientRepository.GetByIdAsync(request.Id, cancellationToken);

        if (existingPatient is null)
            return Result.Failure<Guid>($"Patient with this id: '{request.Id}' doesn't exist");

        // Create Value Objects (Domain Logic handles internal validation, e.g., email format)
        var contactInfo = new ContactInfo(request.Phone, request.Email);
        
        Address? address = null;
        if (!string.IsNullOrWhiteSpace(request.Street) && !string.IsNullOrWhiteSpace(request.City))
        {
            address = new Address(
                request.Street!, 
                request.City!, 
                request.State ?? string.Empty, 
                request.Country ?? string.Empty, 
                request.PostalCode ?? string.Empty);
        }
        
        
        var result = existingPatient.Update(
            request.FirstName, 
            request.LastName, 
            request.DateOfBirth, 
            request.Gender,
            request.EmergencyContact, 
            contactInfo, 
            address);
        
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        
        using var scope = unitOfWork.Begin();
        
        // Persist the Aggregate Root
        await patientRepository.UpdateAsync(existingPatient, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(existingPatient.Id);
    }
}