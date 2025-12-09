using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class CustomerCreditMemoIssuedEventHandler : INotificationHandler<CustomerCreditMemoIssuedEvent>
{
    private readonly IMediator _mediator;

    public CustomerCreditMemoIssuedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(CustomerCreditMemoIssuedEvent notification, CancellationToken cancellationToken)
    {
        var journalEntries = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // Debit: Revenue Adjustment Account (e.g., Sales Returns)
        journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.RevenueAdjustmentAccountId,
            Amount = notification.Amount.Amount,
            IsDebit = true,
            Currency = notification.Amount.Currency
        });

        // Credit: AR Control Account (reduces customer's outstanding balance)
        journalEntries.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.ARControlAccountId,
            Amount = notification.Amount.Amount,
            IsDebit = false,
            Currency = notification.Amount.Currency
        });

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.IssueDate,
            Description = $"Journal entry for customer credit memo {notification.CreditMemoId}. Reason: {notification.Reason}",
            Lines = journalEntries
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}