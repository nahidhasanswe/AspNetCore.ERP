using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsPayable.Events;
using MediatR;
namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class InvoiceIssuedHandler(IMediator mediator) : INotificationHandler<InvoiceIssuedEvent>
{
    public async Task Handle(InvoiceIssuedEvent notification, CancellationToken cancellationToken)
    {
        var ledgerLines = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // 1. Debit the Accounts Receivable control account
        ledgerLines.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.ARControlAccountId,
            Amount = notification.Amount.Amount,
            IsDebit = true, // Debit
            Currency = notification.Amount.Currency,
            CostCenterId = notification.CostCenterId
        });

        // 2. Credit the revenue accounts from each line item
        foreach (var lineItem in notification.LineItems)
        {
            ledgerLines.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = lineItem.RevenueAccountId,
                Amount = lineItem.LineAmount.Amount,
                IsDebit = false, // Credit
                Currency = lineItem.LineAmount.Currency,
                CostCenterId = lineItem.CostCenterId
            });
        }

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.OccurredOn, // Assuming OccurredOn is the posting date
            Description = $"Journal entry for issued customer invoice {notification.InvoiceId}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = ledgerLines
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}