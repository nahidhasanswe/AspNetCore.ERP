using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.UpdateVendorInvoice;

public class UpdateVendorInvoiceCommandHandler(
    IVendorInvoiceRepository invoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateVendorInvoiceCommand, Result>
{
    public async Task<Result> Handle(UpdateVendorInvoiceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure("Invoice not found.");
        }

        var newLineItems = command.NewLineItems.Select(dto => 
            new InvoiceLineItem(dto.Description, dto.LineAmount, dto.ExpenseAccountId, dto.CostCenterId))
            .ToList();

        invoice.Update(command.NewDueDate, newLineItems);

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}