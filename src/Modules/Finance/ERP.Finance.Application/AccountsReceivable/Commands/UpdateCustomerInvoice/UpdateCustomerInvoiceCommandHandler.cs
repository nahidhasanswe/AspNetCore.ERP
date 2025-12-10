using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.UpdateCustomerInvoice;

public class UpdateCustomerInvoiceCommandHandler(
    ICustomerInvoiceRepository invoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateCustomerInvoiceCommand, Result>
{
    public async Task<Result> Handle(UpdateCustomerInvoiceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure("Customer Invoice not found.");
        }

        // No direct change to BusinessUnitId as it's immutable on the aggregate.
        // Validation for BusinessUnitId (e.g., ensuring the invoice belongs to the command's BU)
        // would typically happen here or in a pre-handler.
        if (invoice.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Invoice does not belong to the specified Business Unit.");
        }

        invoice.Update(command.NewDueDate, command.NewCostCenterId);

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}