using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Exceptions;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Commands.HoldVendor;

public class HoldVendorInvoiceCommandHandler(
    IVendorInvoiceRepository repository,
    IUnitOfWorkManager unitOfWork
) : IRequestCommandHandler<HoldVendorInvoiceCommand, bool>
{
    public async Task<Result<bool>> Handle(HoldVendorInvoiceCommand command, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null) throw new NotFoundException("Invoice not found.");

        if (command.PlaceOnHold)
        {
            invoice.PlaceOnHold();
        }
        else
        {
            invoice.RemoveFromHold();
        }

        using var scope = unitOfWork.Begin();
        await repository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken); // Dispatches VendorInvoiceHoldStatusChangedEvent
        
        return Result.Success(true);
    }
}