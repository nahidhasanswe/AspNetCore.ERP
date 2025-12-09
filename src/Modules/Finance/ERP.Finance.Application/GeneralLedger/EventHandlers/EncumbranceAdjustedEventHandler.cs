using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.Encumbrance.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class EncumbranceAdjustedEventHandler : INotificationHandler<EncumbranceAdjustedEvent>
{
    private readonly IMediator _mediator;
    // In a real system, you'd need the Encumbrance Control Account.

    public EncumbranceAdjustedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(EncumbranceAdjustedEvent notification, CancellationToken cancellationToken)
    {
        // We need the Encumbrance Control Account. For now, using a placeholder.
        Guid encumbranceControlAccountId = Guid.NewGuid(); // Placeholder for Encumbrance Control Account

        var journalEntries = new List<CreateJournalEntryCommand.LedgerLineDto>();

        if (notification.AdjustmentAmount.Amount > 0) // Increase in encumbrance
        {
            // Debit Encumbrance Expense, Credit Encumbrance Control
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.GlAccountId,
                Amount = notification.AdjustmentAmount.Amount,
                IsDebit = true,
                Currency = notification.AdjustmentAmount.Currency,
                CostCenterId = notification.CostCenterId
            });
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = encumbranceControlAccountId,
                Amount = notification.AdjustmentAmount.Amount,
                IsDebit = false,
                Currency = notification.AdjustmentAmount.Currency,
                CostCenterId = notification.CostCenterId
            });
        }
        else // Decrease in encumbrance
        {
            // Debit Encumbrance Control, Credit Encumbrance Expense
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = encumbranceControlAccountId,
                Amount = Math.Abs(notification.AdjustmentAmount.Amount),
                IsDebit = true,
                Currency = notification.AdjustmentAmount.Currency,
                CostCenterId = notification.CostCenterId
            });
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.GlAccountId,
                Amount = Math.Abs(notification.AdjustmentAmount.Amount),
                IsDebit = false,
                Currency = notification.AdjustmentAmount.Currency,
                CostCenterId = notification.CostCenterId
            });
        }

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = DateTime.UtcNow,
            Description = $"Journal entry for adjustment of encumbrance {notification.EncumbranceId}",
            Lines = journalEntries
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}