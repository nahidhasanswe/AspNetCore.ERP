using FluentValidation;

namespace ERP.Finance.Application.GeneralLedger.Commands.CreateAccount;

public class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.AccountCode)
            .NotEmpty().WithMessage("Account code is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Account Name is required");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Account type is required");
    }
}