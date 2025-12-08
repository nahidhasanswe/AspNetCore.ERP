using FluentValidation;

namespace ERP.Application.Features.Patient.Commands.RegisterPatiend;

public class RegisterPatientCommandValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        
        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of Birth is required.");
            // Domain rule: Patient must be at least 18 years old to book independently
           // .LessThan(DateTime.UtcNow.Date.AddYears(-18)).WithMessage("Patient must be at least 18 years old.");

        RuleFor(x => x.Gender).NotEmpty().Must(BeValidGender).WithMessage("Gender must be 'Male', 'Female', or 'Other'.");
        
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
    }
    
    private bool BeValidGender(string gender)
    {
        if (string.IsNullOrWhiteSpace(gender)) return false;
        var validGenders = new[] { "Male", "Female", "Other" };
        return validGenders.Contains(gender, StringComparer.OrdinalIgnoreCase);
    }
}