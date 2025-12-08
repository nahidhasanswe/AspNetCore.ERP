using ERP.Core;
using ERP.Core.Exceptions;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Service;
using MediatR;

namespace ERP.Finance.Application.AccountsReceivable.Commands.Deduction;

public class ApplyDeductionCommandHandler(
    ICustomerInvoiceRepository repository,
    IUnitOfWorkManager unitOfWork,
    IDeductionReasonService reasonService // Service to map code to GL account
) : IRequestHandler<ApplyDeductionCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ApplyDeductionCommand command, CancellationToken cancellationToken)
    {
        var invoice = await repository.GetByIdAsync(command.InvoiceId, cancellationToken);
        if (invoice == null) return Result.Failure<bool>($"Invoice {command.InvoiceId} not found.");

        // 1. Resolve GL Expense Account ID (based on the reason code)
        Guid deductionExpenseAccountId = await reasonService.GetExpenseAccountIdByReasonCode(command.DeductionReasonCode, cancellationToken);
        
        if (deductionExpenseAccountId == Guid.Empty)
            return Result.Failure<bool>("Invalid deduction reason code or GL account not configured.");

        // 2. Execute Domain Logic (Requires a new method on CustomerInvoice)
        try
        {
            // The ApplyDeduction method updates status, reduces OutstandingBalance, and raises DeductionAppliedToInvoiceEvent
            invoice.ApplyDeduction(
                command.DeductionAmount, 
                command.DeductionReasonCode, 
                deductionExpenseAccountId
            );
        }
        catch (DomainException ex)
        {
            return Result.Failure<bool>(ex.Message);
        }

        // 3. Persist and Dispatch Event
        using var scope = unitOfWork.Begin();
        await repository.UpdateAsync(invoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken); 
        
        return Result.Success(true);
    }
}