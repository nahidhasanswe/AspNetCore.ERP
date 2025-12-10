using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class InvoiceCancelledEventHandler(IMediator mediator) : INotificationHandler<InvoiceCancelledEvent>
{
    public async Task Handle(InvoiceCancelledEvent notification, CancellationToken cancellationToken)
    {
        var journalEntries = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // 1. Credit the Accounts Receivable Control Account (reversing the original debit)
        journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.ARControlAccountId,
            Amount = notification.TotalAmount.Amount,
            IsDebit = false, // Credit
            Currency = notification.TotalAmount.Currency
        });

        // 2. Debit the original Revenue Accounts from each line item (reversing the original credit)
        foreach (var lineItem in notification.LineItems)
        {
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = lineItem.RevenueAccountId,
                Amount = lineItem.LineAmount.Amount,
                IsDebit = true, // Debit
                Currency = lineItem.LineAmount.Currency,
                CostCenterId = lineItem.CostCenterId
            });
        }

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.CancellationDate,
            Description = $"Journal entry for cancelled customer invoice {notification.InvoiceId}. Reason: {notification.Reason}",
            BusinessUnitId = notification.BusinessUnitId,
            Lines = journalEntries
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}