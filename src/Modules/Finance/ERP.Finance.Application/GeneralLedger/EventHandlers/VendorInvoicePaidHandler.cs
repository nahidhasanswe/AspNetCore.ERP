using ERP.Finance.Application.GeneralLedger.Commands.CreateJournal;
using ERP.Finance.Domain.AccountsPayable.Events;
using MediatR;

namespace ERP.Finance.Application.GeneralLedger.EventHandlers;

public class VendorInvoicePaidHandler(IMediator mediator) : INotificationHandler<VendorPaymentRecordedEvent>
{
    public async Task Handle(VendorPaymentRecordedEvent notification, CancellationToken cancellationToken)
    {
        var createJournalEntryCommand = new CreateJournalEntryCommand
        {
            PostingDate = notification.PaymentDate,
            Description = $"Journal entry for payment of vendor invoice {notification.InvoiceId}",
            BusinessUnitId = notification.BusinessUnitId, // Pass BusinessUnitId
            Lines = new List<CreateJournalEntryCommand.LedgerLineDto>
            {
                // Debit Accounts Payable Control Account
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.APControlAccountId,
                    Amount = notification.AmountPaid.Amount,
                    IsDebit = true,
                    Currency = notification.AmountPaid.Currency,
                    CostCenterId = notification.CostCenterId
                },
                // Credit Payment Account (Cash/Bank)
                new CreateJournalEntryCommand.LedgerLineDto
                {
                    AccountId = notification.PaymentAccountId,
                    Amount = notification.AmountPaid.Amount,
                    IsDebit = false,
                    Currency = notification.AmountPaid.Currency
                }
            }
        };

        await mediator.Send(createJournalEntryCommand, cancellationToken);
    }
}