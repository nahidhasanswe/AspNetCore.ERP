using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Service;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.WriteOff;

public class WriteOffBadDebtCommandHandler(
    ICustomerInvoiceRepository repository,
    IUnitOfWorkManager unitOfWork,
    IGlConfigurationService glConfig // Service to get the Bad Debt GL Account ID
) : IRequestHandler<WriteOffBadDebtCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(WriteOffBadDebtCommand command, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetByIdAsync(command.InvoiceId, cancellationToken);
        
        if (invoice == null) 
            return Result.Failure<bool>($"Invoice {command.InvoiceId} not found.");

        if (invoice.OutstandingBalance.Amount == 0)
            return Result.Failure<bool>("Invoice has no outstanding balance to write off.");

        // 1. Resolve GL Account IDs
        Guid badDebtExpenseAccountId = await glConfig.GetBadDebtExpenseAccountId(invoice.OutstandingBalance.Currency, cancellationToken); 
        
        // 2. Execute Domain Logic (Requires a new method on CustomerInvoice)
        // This method updates the status and raises the BadDebtWrittenOffEvent.
        invoice.WriteOff(command.WriteOffDate, command.Reason, badDebtExpenseAccountId);

        // 3. Persist and Dispatch Event
        using var scope = unitOfWork.Begin();
        await repository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken); 
        
        return Result.Success(true);
    }
}