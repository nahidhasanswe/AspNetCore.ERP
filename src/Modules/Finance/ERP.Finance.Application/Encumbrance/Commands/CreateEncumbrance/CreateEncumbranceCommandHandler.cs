using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Domain.Budgeting.Service;
using ERP.Finance.Domain.Encumbrance.Aggregates;

namespace ERP.Finance.Application.Encumbrance.Commands.CreateEncumbrance;

public class CreateEncumbranceCommandHandler(
    IEncumbranceRepository encumbranceRepository,
    IBudgetRepository budgetRepository,
    IBudgetService budgetService,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateEncumbranceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateEncumbranceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        // 1. Reserve funds in the budget
        var reserveResult = await budgetService.ReserveFunds(
            command.BusinessUnitId,
            command.GlAccountId,
            command.BudgetPeriod,
            command.CostCenterId,
            command.Amount,
            command.SourceTransactionId
        );

        if (reserveResult.IsFailure)
        {
            return Result.Failure<Guid>(reserveResult.Error);
        }

        // Get the updated budget from the service result
        var updatedBudget = reserveResult.Value;

        // 2. Create the Encumbrance aggregate
        var encumbrance = new Domain.Encumbrance.Aggregates.Encumbrance(
            command.BusinessUnitId,
            command.SourceTransactionId,
            command.Amount,
            command.GlAccountId,
            command.CostCenterId
        );

        await encumbranceRepository.AddAsync(encumbrance, cancellationToken);
        await budgetRepository.UpdateAsync(updatedBudget, cancellationToken); // Save the updated budget

        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(encumbrance.Id);
    }
}