using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class InvoiceAdjustedEventHandler(IMediator mediator) : INotificationHandler<InvoiceAdjustedEvent>
{
    public async Task Handle(InvoiceAdjustedEvent notification, CancellationToken cancellationToken)
    {
        var journalEntries = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // If adjustment is positive (increase invoice amount):
        // Debit: AR Control Account
        // Credit: Adjustment Account (e.g., Revenue)
        //
        // If adjustment is negative (decrease invoice amount):
        // Debit: Adjustment Account (e.g., Sales Returns, Discount Expense)
        // Credit: AR Control Account

        if (notification.AdjustmentAmount.Amount > 0)
        {
            // Debit AR Control Account
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.ARControlAccountId,
                Amount = notification.AdjustmentAmount.Amount,
                IsDebit = true,
                Currency = notification.AdjustmentAmount.Currency,
                CostCenterId = notification.CostCenterId
            });
            // Credit Adjustment Account
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.AdjustmentAccountId,
                Amount = notification.AdjustmentAmount.Amount,
                IsDebit = false,
                Currency = notification.AdjustmentAmount.Currency,
                CostCenterId = notification.CostCenterId
            });
        }
        else // Adjustment is negative
        {
            // Debit Adjustment Account
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.AdjustmentAccountId,
                Amount = Math.Abs(notification.AdjustmentAmount.Amount),
                IsDebit = true,
                Currency = notification.AdjustmentAmount.Currency,
                CostCenterId = notification.CostCenterId
            });
            // Credit AR Control Account
            journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = notification.ARControlAccountId,
                Amount = Math.Abs(notification.AdjustmentAmount.Amount),
                IsDebit = false,
                Currency = notification.AdjustmentAmount.Currency,
                CostCenterId = notification.CostCenterId
            });
        }

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.AdjustmentDate,
            Description = $"Journal entry for customer invoice adjustment {notification.InvoiceId}. Reason: {notification.Reason}",
            BusinessUnitId = notification.BusinessUnitId,
            Lines = journalEntries
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}