using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.Encumbrance.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class EncumbranceCanceledEventHandler : INotificationHandler<EncumbranceCanceledEvent>
{
    private readonly IMediator _mediator;
    // In a real system, you'd need the Encumbrance Control Account.

    public EncumbranceCanceledEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(EncumbranceCanceledEvent notification, CancellationToken cancellationToken)
    {
        // This is a reversal of the EncumbranceCreatedEvent's GL entry:
        // Original: Debit Encumbrance Expense, Credit Encumbrance Control
        // Reversal: Debit Encumbrance Control, Credit Encumbrance Expense

        // We need the Encumbrance Control Account. For now, using a placeholder.
        Guid encumbranceControlAccountId = Guid.NewGuid(); // Placeholder for Encumbrance Control Account

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = DateTime.UtcNow,
            Description = $"Journal entry for canceled encumbrance {notification.EncumbranceId}",
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // Debit Encumbrance Control Account (reversing the original credit)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = encumbranceControlAccountId,
                    Amount = notification.Amount.Amount,
                    IsDebit = true,
                    Currency = notification.Amount.Currency,
                    CostCenterId = notification.CostCenterId
                },
                // Credit Encumbrance Expense Account (reversing the original debit)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.GlAccountId,
                    Amount = notification.Amount.Amount,
                    IsDebit = false,
                    Currency = notification.Amount.Currency,
                    CostCenterId = notification.CostCenterId
                }
            }
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}