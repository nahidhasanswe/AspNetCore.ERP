using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using ERP.Shared.Events.Events;

namespace ERP.Finance.Application.AccountsPayable.EventHandlers;

public class BatchInvoicePaymentEventHandler(IVendorInvoiceRepository invoiceRepository, IUnitOfWorkManager unitOfWork)
    : INotificationHandler<BatchInvoicePaymentEvent>
{
    public async Task Handle(BatchInvoicePaymentEvent notification, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoice = await invoiceRepository.GetByIdAsync(notification.InvoiceId, cancellationToken);
        if (invoice is not null)
        {
            // The AP Control Account ID is retrieved from the invoice itself
            invoice.RecordPayment(
                new Money(notification.PaymentAmount, notification.PaymentAmountCurrency),
                notification.TransactionReference,
                notification.PaymentDate,
                notification.PaymentAccountId,
                invoice.APControlAccountId
            );

            await invoiceRepository.UpdateAsync(invoice, cancellationToken);
            await scope.SaveChangesAsync(cancellationToken);
        }
        // If invoice is null, log an error.
    }
}