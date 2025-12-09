using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class CashReceiptBatchPostedEventHandler : INotificationHandler<CashReceiptBatchPostedEvent>
{
    private readonly IMediator _mediator;
    // In a real system, you'd need the AR Control Account.

    public CashReceiptBatchPostedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(CashReceiptBatchPostedEvent notification, CancellationToken cancellationToken)
    {
        // GL Entry for a posted cash receipt batch:
        // Debit: Cash Account (Bank Account)
        // Credit: AR Control Account (or Unapplied Cash Account if not yet applied to invoices)

        // We need the AR Control Account. For now, using a placeholder.
        Guid arControlAccountId = Guid.NewGuid(); // Placeholder for AR Control Account

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.BatchDate,
            Description = $"Journal entry for cash receipt batch {notification.BatchId}. Reference: {notification.Reference}",
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // Debit Cash Account (Bank Account)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.CashAccountId,
                    Amount = notification.TotalBatchAmount.Amount,
                    IsDebit = true,
                    Currency = notification.TotalBatchAmount.Currency
                },
                // Credit AR Control Account (or Unapplied Cash Account)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = arControlAccountId, // This needs to be the AR Control Account
                    Amount = notification.TotalBatchAmount.Amount,
                    IsDebit = false,
                    Currency = notification.TotalBatchAmount.Currency
                }
            }
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}