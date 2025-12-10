using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.Encumbrance.Events;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class EncumbranceGlPostingHandler(IMediator mediator) : INotificationHandler<EncumbranceCreatedEvent>
{
    public async Task Handle(EncumbranceCreatedEvent notification, CancellationToken cancellationToken)
    {
        // We need the Encumbrance Control Account and a Budgetary Appropriation Account.
        // For now, using placeholders.
        Guid encumbranceControlAccountId = Guid.NewGuid(); // Placeholder for Encumbrance Control Account
        Guid budgetaryAppropriationAccountId = Guid.NewGuid(); // Placeholder for Budgetary Appropriation Account

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.OccurredOn, // Assuming OccurredOn is the posting date
            Description = $"Journal entry for encumbrance {notification.EncumbranceId} ({notification.Type})",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // Debit Encumbrance Control Account
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = encumbranceControlAccountId,
                    Amount = notification.Amount.Amount,
                    IsDebit = true,
                    Currency = notification.Amount.Currency,
                    CostCenterId = notification.CostCenterId
                },
                // Credit Budgetary Appropriation Account
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = budgetaryAppropriationAccountId,
                    Amount = notification.Amount.Amount,
                    IsDebit = false,
                    Currency = notification.Amount.Currency,
                    CostCenterId = notification.CostCenterId
                }
            }
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}