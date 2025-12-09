using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;

namespace ERP.Finance.Application.Budgeting.Commands.ActivateBudget;

public class ActivateBudgetCommandHandler(IBudgetRepository budgetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<ActivateBudgetCommand, Guid>
{
    public async Task<Result<Guid>> Handle(ActivateBudgetCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var budget = await budgetRepository.GetByIdAsync(command.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure<Guid>("Budget not found.");
        }

        budget.Activate();

        await budgetRepository.UpdateAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(budget.Id);
    }
}