using ERP.Domain.Aggregates.ClinicAggregate;
using FluentValidation;

namespace ERP.Application.Features.Clinics.Commands.CreateClinic;

public sealed class CreateClinicCommandValidator: AbstractValidator<CreateClinicCommand>
{
    private readonly IClinicRepository _clinicRepository;
    public CreateClinicCommandValidator(IClinicRepository clinicRepository)
    {
        _clinicRepository = clinicRepository;
        
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(100);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
    }
}