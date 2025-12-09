using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.Encumbrance.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class EncumbranceLiquidatedEventHandler : INotificationHandler<EncumbranceLiquidatedEvent>
{
    private readonly IMediator _mediator;
    // In a real system, you'd need the Encumbrance Control Account.

    public EncumbranceLiquidatedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(EncumbranceLiquidatedEvent notification, CancellationToken cancellationToken)
    {
        // This event signifies that an actual expenditure has occurred against an encumbrance.
        // The GL entry should reverse the encumbrance and recognize the actual expense.
        // The actual expense is typically recorded by another event (e.g., InvoiceApprovedEvent).
        // This handler's job is to reverse the encumbrance entry.

        // We need the Encumbrance Control Account. For now, using a placeholder.
        Guid encumbranceControlAccountId = Guid.NewGuid(); // Placeholder for Encumbrance Control Account

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = DateTime.UtcNow,
            Description = $"Journal entry for liquidated encumbrance {notification.EncumbranceId} by transaction {notification.ActualTransactionId}",
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