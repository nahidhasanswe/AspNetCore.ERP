using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using MediatR;

namespace ERP.Finance.Application.AccountsPayable.Commands.DeactivateRecurringInvoice;

public class DeactivateRecurringInvoiceCommandHandler(
    IRecurringInvoiceRepository recurringInvoiceRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<DeactivateRecurringInvoiceCommand, Unit>
{
    public async Task<Result<Unit>> Handle(DeactivateRecurringInvoiceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var recurringInvoice = await recurringInvoiceRepository.GetByIdAsync(command.RecurringInvoiceId, cancellationToken);
        if (recurringInvoice == null)
        {
            return Result.Failure<Unit>("Recurring Invoice not found.");
        }

        recurringInvoice.Deactivate();

        await recurringInvoiceRepository.UpdateAsync(recurringInvoice, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}