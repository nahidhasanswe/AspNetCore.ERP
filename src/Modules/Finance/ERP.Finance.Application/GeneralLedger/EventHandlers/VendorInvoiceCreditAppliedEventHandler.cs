using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsPayable.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class VendorInvoiceCreditAppliedEventHandler : INotificationHandler<VendorInvoiceCreditAppliedEvent>
{
    private readonly IMediator _mediator;
    // In a real system, you'd inject a configuration service to get the Vendor Credits account
    // private readonly IConfigurationService _configService; 

    public VendorInvoiceCreditAppliedEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(VendorInvoiceCreditAppliedEvent notification, CancellationToken cancellationToken)
    {
        // Guid vendorCreditsAccountId = await _configService.GetVendorCreditsAccountId();
        Guid vendorCreditsAccountId = Guid.NewGuid(); // Placeholder

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.AppliedDate,
            Description = $"Applied credit memo {notification.CreditMemoId} to invoice {notification.InvoiceId}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // 1. Debit the Accounts Payable control account to reduce liability
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.APControlAccountId,
                    Amount = notification.AmountApplied.Amount,
                    IsDebit = true,
                    Currency = notification.AmountApplied.Currency
                },
                // 2. Credit a "Vendor Credits" clearing account
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = vendorCreditsAccountId,
                    Amount = notification.AmountApplied.Amount,
                    IsDebit = false,
                    Currency = notification.AmountApplied.Currency
                }
            }
        };

        await _mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}