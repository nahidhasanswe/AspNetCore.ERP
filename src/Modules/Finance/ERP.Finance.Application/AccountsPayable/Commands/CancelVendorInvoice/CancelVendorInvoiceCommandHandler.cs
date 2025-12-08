using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsPayable.Commands.CancelVendorInvoice;

public class CancelVendorInvoiceCommandHandler(
    IVendorInvoiceRepository invoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CancelVendorInvoiceCommand, bool>
{
    public async Task<Result<bool>> Handle(CancelVendorInvoiceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure<bool>("Invoice not found.");
        }

        invoice.Cancel(command.Reason);

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}