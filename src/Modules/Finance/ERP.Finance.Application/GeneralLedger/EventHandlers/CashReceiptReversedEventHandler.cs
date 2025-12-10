using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class CashReceiptReversedEventHandler : INotificationHandler<CashReceiptReversedEvent>
{
    private readonly IMediator _mediator;
    // In a real system, you'd need the AR Control Account.

    public CashReceiptReversedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(CashReceiptReversedEvent notification, CancellationToken cancellationToken)
    {
        // This event signifies a full reversal of the cash receipt.
        // It needs to reverse the initial GL entry (Debit Cash, Credit AR Control)
        // AND any subsequent application entries (Debit AR Control, Credit Cash for payments).
        // This is complex and often requires looking up original transactions.

        // For simplicity, this handler will reverse the initial cash receipt entry.
        // Reversing applications would typically be handled by unapplying payments/credits first.

        // We need the AR Control Account. For now, using a placeholder.
        Guid arControlAccountId = Guid.NewGuid(); // Placeholder for AR Control Account

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.ReversalDate,
            Description = $"Journal entry for reversal of cash receipt {notification.ReceiptId}. Reason: {notification.Reason}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // Debit AR Control Account (reversing the original credit from cash receipt)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = arControlAccountId, // This needs to be the AR Control Account
                    Amount = notification.TotalReceivedAmount.Amount,
                    IsDebit = true,
                    Currency = notification.TotalReceivedAmount.Currency
                },
                // Credit Cash Account (reversing the original debit)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.CashAccountId,
                    Amount = notification.TotalReceivedAmount.Amount,
                    IsDebit = false,
                    Currency = notification.TotalReceivedAmount.Currency
                }
            }
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}