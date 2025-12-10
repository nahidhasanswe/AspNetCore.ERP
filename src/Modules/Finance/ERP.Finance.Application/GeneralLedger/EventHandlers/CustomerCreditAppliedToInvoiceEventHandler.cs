using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsReceivable.Events;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class CustomerCreditAppliedToInvoiceEventHandler : INotificationHandler<CustomerCreditAppliedToInvoiceEvent>
{
    private readonly IMediator _mediator;
    // In a real system, you'd inject a configuration service to get the Customer Credits Clearing account
    // private readonly IConfigurationService _configService; 

    public CustomerCreditAppliedToInvoiceEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(CustomerCreditAppliedToInvoiceEvent notification, CancellationToken cancellationToken)
    {
        // Guid customerCreditsClearingAccountId = await _configService.GetCustomerCreditsClearingAccountId();
        Guid customerCreditsClearingAccountId = Guid.NewGuid(); // Placeholder

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.AppliedDate,
            Description = $"Applied customer credit memo {notification.CreditMemoId} to invoice {notification.InvoiceId}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // 1. Debit the AR Control Account (reduces AR liability)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.ARControlAccountId,
                    Amount = notification.AmountApplied.Amount,
                    IsDebit = true,
                    Currency = notification.AmountApplied.Currency
                },
                // 2. Credit a "Customer Credits Clearing" account (reversing the initial debit to this account when memo was issued)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = customerCreditsClearingAccountId, // This would be the same account credited when the memo was issued
                    Amount = notification.AmountApplied.Amount,
                    IsDebit = false,
                    Currency = notification.AmountApplied.Currency
                }
            }
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}