using ERP.Core;
using ERP.Core.Uow;
using ERP.Domain.Aggregates.PatientAggregate;
using ERP.Domain.ValueObjects;
using MediatR;

namespace ERP.Application.Features.Patient.Commands.RegisterPatiend;

public class RegisterPatientCommandHandler(IPatientRepository patientRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<RegisterPatientCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterPatientCommand request, CancellationToken cancellationToken)
    {
        // Application Rule: Check for uniqueness (using the repository for persistence checks)
        var emailExists = await patientRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (emailExists)
            return Result.Failure<Guid>("Patient already exists with this email address.");

        var phoneExists = await patientRepository.ExistsByPhoneAsync(request.Phone, cancellationToken);
        if (phoneExists)
            return Result.Failure<Guid>("Patient already exists with this phone number.");

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

        // Create Aggregate Root (Domain Logic starts here)
        var patient = new Domain.Aggregates.PatientAggregate.Patient(
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Gender,
            contactInfo,
            address,
            request.EmergencyContact);

        using var scope = unitOfWork.Begin();
        
        // Persist the Aggregate Root
        await patientRepository.AddAsync(patient, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(patient.Id);
    }
}