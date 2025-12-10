using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class CashReceiptCancelledEventHandler(IMediator mediator) : INotificationHandler<CashReceiptCancelledEvent>
{
    public async Task Handle(CashReceiptCancelledEvent notification, CancellationToken cancellationToken)
    {
        // This is a reversal of the UnappliedCashCreatedEvent's GL entry:
        // Original: Debit Cash, Credit Unapplied Cash (or AR Control)
        // Reversal: Debit Unapplied Cash (or AR Control), Credit Cash

        // Assuming UnappliedCashCreatedEvent debits CashAccountId and credits ARControlAccountId
        // So, reversal would be: Debit ARControlAccountId, Credit CashAccountId

        // We need the ARControlAccountId. For now, using a placeholder.
        Guid arControlAccountId = Guid.NewGuid(); // Placeholder for AR Control Account

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.CancellationDate,
            Description = $"Journal entry for cancelled cash receipt {notification.ReceiptId}. Reference: {notification.TransactionReference}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // Debit AR Control Account (reversing the original credit)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = arControlAccountId, // This needs to be the AR Control Account
                    Amount = notification.Amount.Amount,
                    IsDebit = true,
                    Currency = notification.Amount.Currency
                },
                // Credit Cash Account (reversing the original debit)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.CashAccountId,
                    Amount = notification.Amount.Amount,
                    IsDebit = false,
                    Currency = notification.Amount.Currency
                }
            }
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}