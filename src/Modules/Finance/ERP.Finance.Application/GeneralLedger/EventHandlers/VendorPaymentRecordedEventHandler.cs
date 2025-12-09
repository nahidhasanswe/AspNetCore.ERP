using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsPayable.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class VendorPaymentRecordedEventHandler : INotificationHandler<VendorPaymentRecordedEvent>
{
    private readonly IMediator _mediator;

    public VendorPaymentRecordedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(VendorPaymentRecordedEvent notification, CancellationToken cancellationToken)
    {
        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.PaymentDate,
            Description = $"Record payment for invoice {notification.InvoiceId} via {notification.TransactionReference}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // 1. Debit the Accounts Payable control account to reduce liability
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.APControlAccountId,
                    Amount = notification.AmountPaid.Amount,
                    IsDebit = true, // Debit
                    Currency = notification.AmountPaid.Currency,
                    CostCenterId = notification.CostCenterId
                },
                // 2. Credit the cash/bank account from which payment was made
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.PaymentAccountId,
                    Amount = notification.AmountPaid.Amount,
                    IsDebit = false, // Credit
                    Currency = notification.AmountPaid.Currency
                }
            }
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}