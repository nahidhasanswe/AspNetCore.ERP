using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Exceptions;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Commands.CancelVendor;

public class CancelVendorInvoiceCommandHandler(
    IVendorInvoiceRepository repository,
    IUnitOfWorkManager unitOfWork
) : IRequestCommandHandler<CancelVendorInvoiceCommand, bool>
{
    public async Task<Result<bool>> Handle(CancelVendorInvoiceCommand command, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null) throw new NotFoundException("Invoice not found.");

        invoice.Cancel(command.CancellationReason);

        using var scope = unitOfWork.Begin();
        await repository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken); // Dispatches VendorInvoiceCancelledEvent
        
        return Result.Success(true);
    }
}