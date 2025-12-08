using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Exceptions;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;

namespace ERP.Finance.Application.AccountsPayable.Commands.UpdateVendor;

public class UpdateVendorInvoiceCommandHandler(
    IVendorInvoiceRepository repository,
    IUnitOfWorkManager unitOfWork
) : IRequestCommandHandler<UpdateVendorInvoiceCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateVendorInvoiceCommand command, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null) throw new NotFoundException("Invoice not found.");
        
        // Map DTOs back to Domain Objects (InvoiceLineItem)
        var newLines = command.UpdatedLines.Select(dto => 
            new InvoiceLineItem(
                dto.Description,
                dto.LineAmount,
                dto.ExpenseAccountId,
                dto.CostCenterId
            )).ToList();
        
        // Execute Domain Logic
        invoice.Update(command.NewDueDate, newLines);

        using var scope = unitOfWork.Begin();
        await repository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
        
        return Result.Success(true);
    }
}