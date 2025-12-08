using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Commands.PlaceInvoiceOnHold;

public class PlaceInvoiceOnHoldCommandHandler(IVendorInvoiceRepository invoiceRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<PlaceInvoiceOnHoldCommand, bool>
{
    public async Task<Result<bool>> Handle(PlaceInvoiceOnHoldCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure<bool>("Invoice not found.");
        }

        invoice.PlaceOnHold();

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}