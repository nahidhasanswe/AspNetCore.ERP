using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.Encumbrance.Events;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class EncumbranceCommitmentGlPostingHandler(IMediator mediator)
    : INotificationHandler<EncumbranceConvertedToCommitmentEvent>
{
    public async Task Handle(EncumbranceConvertedToCommitmentEvent notification, CancellationToken cancellationToken)
    {
        // This handler is for the GL impact when an encumbrance (reservation) becomes a firm commitment (e.g., PO issued).
        // It typically involves adjusting the encumbrance amount if it changed, and reclassifying.
        // For simplicity, we'll assume a direct reclassification or adjustment of the original encumbrance.

        // We need the Encumbrance Control Account and a Commitment Control Account.
        // For now, using placeholders.
        Guid encumbranceControlAccountId = Guid.NewGuid(); // Placeholder for Encumbrance Control Account
        Guid commitmentControlAccountId = Guid.NewGuid(); // Placeholder for Commitment Control Account

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.OccurredOn, // Assuming OccurredOn is the posting date
            Description = $"Journal entry for encumbrance {notification.EncumbranceId} converted to commitment",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // Debit Encumbrance Control Account (reversing the original reservation)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = encumbranceControlAccountId,
                    Amount = notification.FinalCommittedAmount.Amount,
                    IsDebit = true,
                    Currency = notification.FinalCommittedAmount.Currency,
                    CostCenterId = notification.CostCenterId
                },
                // Credit Commitment Control Account (recognizing the firm commitment)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = commitmentControlAccountId,
                    Amount = notification.FinalCommittedAmount.Amount,
                    IsDebit = false,
                    Currency = notification.FinalCommittedAmount.Currency,
                    CostCenterId = notification.CostCenterId
                }
            }
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}