using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Commands.RemoveInvoiceFromHold;

public class RemoveInvoiceFromHoldCommandHandler(
    IVendorInvoiceRepository invoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<RemoveInvoiceFromHoldCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RemoveInvoiceFromHoldCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure<Guid>("Invoice not found.");
        }

        invoice.RemoveFromHold();

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(invoice.Id);
    }
}