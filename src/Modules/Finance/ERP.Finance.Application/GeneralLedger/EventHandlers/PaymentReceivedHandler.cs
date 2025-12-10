using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class PaymentReceivedHandler(IMediator mediator) : INotificationHandler<PaymentReceivedEvent>
{
    public async Task Handle(PaymentReceivedEvent notification, CancellationToken cancellationToken)
    {
        var ledgerLines = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // 1. Debit the Cash/Bank account
        ledgerLines.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.CashAccountId,
            Amount = notification.AmountReceived.Amount,
            IsDebit = true, // Debit
            Currency = notification.AmountReceived.Currency,
            CostCenterId = notification.CostCenterId
        });

        // 2. Credit the Accounts Receivable control account
        ledgerLines.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.ArControlAccountId,
            Amount = notification.AmountReceived.Amount,
            IsDebit = false, // Credit
            Currency = notification.AmountReceived.Currency,
            CostCenterId = notification.CostCenterId
        });

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.PaymentDate,
            Description = $"Journal entry for payment received for invoice {notification.InvoiceId} via {notification.Reference}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = ledgerLines
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}