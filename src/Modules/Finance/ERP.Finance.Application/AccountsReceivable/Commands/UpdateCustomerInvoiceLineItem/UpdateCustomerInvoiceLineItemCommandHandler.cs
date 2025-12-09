using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Commands.UpdateCustomerInvoiceLineItem;

public class UpdateCustomerInvoiceLineItemCommandHandler(
    ICustomerInvoiceRepository invoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateCustomerInvoiceLineItemCommand, Result>
{
    public async Task<Result> Handle(UpdateCustomerInvoiceLineItemCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure("Customer Invoice not found.");
        }

        invoice.UpdateLineItem(
            command.LineItemId,
            command.Description,
            command.LineAmount,
            command.RevenueAccountId,
            command.CostCenterId
        );

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}