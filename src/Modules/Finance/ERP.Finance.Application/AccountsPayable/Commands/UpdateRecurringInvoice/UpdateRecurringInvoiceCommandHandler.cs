using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.UpdateRecurringInvoice;

public class UpdateRecurringInvoiceCommandHandler(
    IRecurringInvoiceRepository recurringInvoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<UpdateRecurringInvoiceCommand, Unit>
{
    public async Task<Result<Unit>> Handle(UpdateRecurringInvoiceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var recurringInvoice = await recurringInvoiceRepository.GetByIdAsync(command.RecurringInvoiceId, cancellationToken);
        if (recurringInvoice == null)
        {
            return Result.Failure<Unit>("Recurring Invoice not found.");
        }

        var newLines = command.Lines.Select(dto => new RecurringInvoiceLine
        (
            dto.Description,
            dto.LineAmount,
            dto.ExpenseAccountId,
            dto.CostCenterId
        )).ToList();

        recurringInvoice.Update(command.Interval, command.StartDate, command.EndDate, newLines);

        await recurringInvoiceRepository.UpdateAsync(recurringInvoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}