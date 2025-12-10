using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class BadDebtGeneralLedgerPostingHandler(IMediator mediator) : INotificationHandler<BadDebtWrittenOffEvent>
{
    public async Task Handle(BadDebtWrittenOffEvent notification, CancellationToken cancellationToken)
    {
        var ledgerLines = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // 1. Debit Bad Debt Expense Account
        ledgerLines.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.BadDebtExpenseAccountId,
            Amount = notification.WriteOffAmount.Amount,
            IsDebit = true, // Debit
            Currency = notification.WriteOffAmount.Currency,
            CostCenterId = notification.CostCenterId
        });

        // 2. Credit AR Control Account
        ledgerLines.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.ARControlAccountId,
            Amount = notification.WriteOffAmount.Amount,
            IsDebit = false, // Credit
            Currency = notification.WriteOffAmount.Currency,
            CostCenterId = notification.CostCenterId
        });

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.WriteOffDate,
            Description = $"Journal entry for bad debt write-off for invoice {notification.InvoiceId}. Reason: {notification.Reason}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = ledgerLines
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}