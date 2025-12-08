using ERP.Domain.Aggregates.ClinicAggregate;
using FluentValidation;

namespace ERP.Application.Features.Clinics.Commands.UpdateClinic;

public sealed class UpdateClinicCommandValidator: AbstractValidator<UpdateClinicCommand>
{
    private readonly IClinicRepository _clinicRepository;
    
    public UpdateClinicCommandValidator(IClinicRepository clinicRepository)
    {
        _clinicRepository = clinicRepository;
        
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(100);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        
        RuleFor(x => x.Id).NotEmpty().MustAsync(BeAnExistingClinicAsync)
            .WithMessage("A clinic with the provided id does not exist.");;
    }

    private async Task<bool> BeAnExistingClinicAsync(Guid clinicId, CancellationToken cancellationToken)
    {
        return  !(await _clinicRepository.ExistsAsync(clinicId, cancellationToken));
    }
}