using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Commands.RecordVendorPayment;

public class RecordVendorPaymentCommandHandler(
    IVendorInvoiceRepository invoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<RecordVendorPaymentCommand, bool>
{
    public async Task<Result<bool>> Handle(RecordVendorPaymentCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();
        
        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure<bool>("Invoice not found.");
        }

        // The AP Control Account ID should be retrieved from the invoice itself
        invoice.RecordPayment(
            command.PaymentAmount,
            command.TransactionReference,
            command.PaymentDate,
            command.PaymentAccountId,
            invoice.APControlAccountId 
        );

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}