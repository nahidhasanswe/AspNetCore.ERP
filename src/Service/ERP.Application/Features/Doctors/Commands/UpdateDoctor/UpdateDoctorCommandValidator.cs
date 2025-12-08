using ERP.Domain.Aggregates.ClinicAggregate;
using ERP.Domain.Aggregates.DoctorAggregate;
using FluentValidation;

namespace ERP.Application.Features.Doctors.Commands.UpdateDoctor;

public sealed class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IClinicRepository _clinicRepository;

    public UpdateDoctorCommandValidator(IDoctorRepository doctorRepository, IClinicRepository clinicRepository)
    {
        _doctorRepository = doctorRepository;
        _clinicRepository = clinicRepository;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Doctor ID is required.")
            .MustAsync(DoctorMustExistAsync).WithMessage("Doctor not found.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Specialization)
            .NotEmpty().WithMessage("Specialization is required.")
            .MaximumLength(100).WithMessage("Specialization cannot exceed 100 characters.");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("License number is required.")
            .MaximumLength(30).WithMessage("License number cannot exceed 30 characters.")
            .MustAsync(LicenseNumberMustBeUniqueAsync).WithMessage("Another doctor is already registered with this license number.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email address cannot exceed 100 characters.");

        RuleFor(x => x.ClinicId)
            .NotEmpty().WithMessage("Clinic ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Clinic ID must be a valid, non-empty GUID.")
            .MustAsync(ClinicMustExistAsync).WithMessage("Clinic doesn't exist.");
    }

    private async Task<bool> ClinicMustExistAsync(Guid clinicId, CancellationToken cancellationToken)
    {
        return !await _clinicRepository.ExistsAsync(clinicId, cancellationToken);
    }

    private async Task<bool> DoctorMustExistAsync(Guid doctorId, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
        return doctor is not null;
    }

    private async Task<bool> LicenseNumberMustBeUniqueAsync(string licenseNumber, CancellationToken cancellationToken)
    {
        return !await _doctorRepository.ExistsByLicenseNumberAsync(licenseNumber, cancellationToken);
    }
}
