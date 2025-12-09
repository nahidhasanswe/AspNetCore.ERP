using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class PaymentUnappliedEventHandler : INotificationHandler<PaymentUnappliedEvent>
{
    private readonly IMediator _mediator;
    // In a real system, you'd need to retrieve the original CashAccountId from the PaymentReceivedEvent
    // For simplicity, we'll assume a generic Cash Account for reversal.

    public PaymentUnappliedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(PaymentUnappliedEvent notification, CancellationToken cancellationToken)
    {
        // This is a reversal of the PaymentReceivedEvent's GL entry:
        // Original: Debit Cash, Credit AR Control
        // Reversal: Debit AR Control, Credit Cash

        // You would need to find the original CashAccountId from the payment record.
        // For now, using a placeholder.
        Guid cashAccountId = Guid.NewGuid(); // Placeholder for the original cash account

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.UnappliedDate,
            Description = $"Journal entry for unapplied payment from invoice {notification.InvoiceId}",
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // Debit AR Control Account (increases AR liability)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.ARControlAccountId,
                    Amount = notification.AmountUnapplied.Amount,
                    IsDebit = true,
                    Currency = notification.AmountUnapplied.Currency
                },
                // Credit Cash Account (reduces cash)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = cashAccountId, // This needs to be the original cash account
                    Amount = notification.AmountUnapplied.Amount,
                    IsDebit = false,
                    Currency = notification.AmountUnapplied.Currency
                }
            }
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}