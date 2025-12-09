using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsPayable.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class VendorInvoiceApprovedEventHandler : INotificationHandler<VendorInvoiceApprovedEvent>
{
    private readonly IMediator _mediator;

    public VendorInvoiceApprovedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(VendorInvoiceApprovedEvent notification, CancellationToken cancellationToken)
    {
        var ledgerLines = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // 1. Credit the Accounts Payable control account
        ledgerLines.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.APControlAccountId,
            Amount = notification.TotalAmount.Amount,
            IsDebit = false, // Credit
            Currency = notification.TotalAmount.Currency
        });

        // 2. Debit the expense accounts from each line item
        foreach (var lineItem in notification.LineItems)
        {
            ledgerLines.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = lineItem.ExpenseAccountId,
                Amount = lineItem.LineAmount.Amount,
                IsDebit = true, // Debit
                Currency = lineItem.LineAmount.Currency,
                CostCenterId = lineItem.CostCenterId
            });
        }

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.ApprovalDate,
            Description = $"Journal entry for approved invoice {notification.InvoiceId}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = ledgerLines
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}