using FluentValidation;

namespace ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;

public class CreateJournalValidator : AbstractValidator<CreateJournalEntryCommand>
{
    public CreateJournalValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required");

        RuleFor(x => x.ReferenceNumber)
            .NotEmpty().WithMessage("Reference Number is required");

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("Cannot post an entry with no ledger lines.");
    }
}