using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.UpdateBudgetItem;

public class UpdateBudgetItemCommandHandler(IBudgetRepository budgetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateBudgetItemCommand, Result>
{
    public async Task<Result> Handle(UpdateBudgetItemCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var budget = await budgetRepository.GetByIdAsync(command.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure("Budget not found.");
        }

        budget.UpdateItem(command.BudgetItemId, command.NewBudgetedAmount, command.NewPeriod, command.NewCostCenterId);

        await budgetRepository.UpdateAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}