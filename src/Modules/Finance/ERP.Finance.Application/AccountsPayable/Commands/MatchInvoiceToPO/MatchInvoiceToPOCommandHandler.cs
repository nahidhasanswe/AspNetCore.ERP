using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.MatchInvoiceToPO;

public class MatchInvoiceToPoCommandHandler(
    IVendorInvoiceRepository invoiceRepository,
    IPurchaseOrderRepository poRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<MatchInvoiceToPOCommand, Unit>
{
    public async Task<Result<Unit>> Handle(MatchInvoiceToPOCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure<Unit>("Invoice not found.");
        }

        var purchaseOrder = await poRepository.GetByIdAsync(command.PurchaseOrderId, cancellationToken);
        if (purchaseOrder == null)
        {
            return Result.Failure<Unit>("Purchase Order not found.");
        }

        invoice.MatchToPO(purchaseOrder, command.Perform3WayMatch);

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}