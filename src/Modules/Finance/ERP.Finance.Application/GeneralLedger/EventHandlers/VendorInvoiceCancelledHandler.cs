using ERP.Finance.Domain.AccountsPayable.Events;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using MediatR;
using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class VendorInvoiceCancelledHandler(
    IFiscalPeriodRepository fiscalPeriodRepository,
    IMediator mediator)
    : INotificationHandler<VendorInvoiceCancelledEvent>
{
    private const string SystemBaseCurrency = "USD";

    public async Task Handle(VendorInvoiceCancelledEvent notification, CancellationToken cancellationToken)
    {
        var fiscalPeriod = await fiscalPeriodRepository.GetPeriodByDateAsync(notification.CancellationDate, cancellationToken);
        if (fiscalPeriod is null)
        {
            // Log error: Cannot post invoice cancellation GL as no open fiscal period was found.
            return;
        }

        var ledgerLines = new List<CreateJournalEntryCommand.LedgerLineDto>();

        // 1. Debit the Accounts Payable control account to reverse liability
        ledgerLines.Add(new CreateJournalEntryCommand.LedgerLineDto
        {
            AccountId = notification.OriginalApControlAccountId,
            Amount = notification.OriginalTotalAmount.Amount,
            IsDebit = true, // Debit
            Currency = notification.OriginalTotalAmount.Currency
        });

        // 2. Credit the original expense accounts from each line item to reverse expense
        foreach (var lineItem in notification.OriginalLineItems)
        {
            ledgerLines.Add(new CreateJournalEntryCommand.LedgerLineDto
            {
                AccountId = lineItem.ExpenseAccountId,
                Amount = lineItem.LineAmount.Amount,
                IsDebit = false, // Credit
                Currency = lineItem.LineAmount.Currency,
                CostCenterId = lineItem.CostCenterId
            });
        }

        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.CancellationDate,
            Description = $"Journal entry for cancelled invoice {notification.InvoiceId}. Reason: {notification.CancellationReason}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = ledgerLines
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}