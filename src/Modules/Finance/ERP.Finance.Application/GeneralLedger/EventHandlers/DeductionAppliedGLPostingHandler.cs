using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class DeductionAppliedGlPostingHandler(IMediator mediator) : INotificationHandler<DeductionAppliedToInvoiceEvent>
{
    public async Task Handle(DeductionAppliedToInvoiceEvent notification, CancellationToken cancellationToken)
    {
        var ledgerLines = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // 1. Debit Deduction Expense Account
        ledgerLines.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.DeductionExpenseAccountId,
            Amount = notification.DeductionAmount.Amount,
            IsDebit = true, // Debit
            Currency = notification.DeductionAmount.Currency
        });

        // 2. Credit AR Control Account
        ledgerLines.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.ARControlAccountId,
            Amount = notification.DeductionAmount.Amount,
            IsDebit = false, // Credit
            Currency = notification.DeductionAmount.Currency
        });

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.OccurredOn, // Assuming OccurredOn is the posting date
            Description = $"Journal entry for deduction applied to invoice {notification.InvoiceId}. Reason: {notification.DeductionReasonCode}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = ledgerLines
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}