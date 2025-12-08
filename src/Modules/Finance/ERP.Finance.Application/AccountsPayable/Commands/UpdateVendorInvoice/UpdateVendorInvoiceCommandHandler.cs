using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.UpdateVendorInvoice;

public class UpdateVendorInvoiceCommandHandler(
    IVendorInvoiceRepository invoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<UpdateVendorInvoiceCommand, Unit>
{
    public async Task<Result<Unit>> Handle(UpdateVendorInvoiceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null)
        {
            return Result.Failure<Unit>("Invoice not found.");
        }

        var newLineItems = command.NewLineItems.Select(dto => 
            new InvoiceLineItem(dto.Description, dto.LineAmount, dto.ExpenseAccountId, dto.CostCenterId))
            .ToList();

        invoice.Update(command.NewDueDate, newLineItems);

        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}