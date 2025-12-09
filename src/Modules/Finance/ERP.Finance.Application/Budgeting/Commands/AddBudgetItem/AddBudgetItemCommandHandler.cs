using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;

namespace ERP.Finance.Application.Budgeting.Commands.AddBudgetItem;

public class AddBudgetItemCommandHandler(IBudgetRepository budgetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<AddBudgetItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddBudgetItemCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var budget = await budgetRepository.GetByIdAsync(command.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure<Guid>("Budget not found.");
        }

        var budgetItem = new BudgetItem(
            command.AccountId,
            command.BudgetedAmount,
            command.Period,
            command.CostCenterId
        );

        budget.AddItem(budgetItem);

        await budgetRepository.UpdateAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(budgetItem.Id);
    }
}