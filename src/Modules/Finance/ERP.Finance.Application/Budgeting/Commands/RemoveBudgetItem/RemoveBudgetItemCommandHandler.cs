using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.RemoveBudgetItem;

public class RemoveBudgetItemCommandHandler(IBudgetRepository budgetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<RemoveBudgetItemCommand, Result>
{
    public async Task<Result> Handle(RemoveBudgetItemCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var budget = await budgetRepository.GetByIdAsync(command.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure("Budget not found.");
        }

        budget.RemoveItem(command.BudgetItemId);

        await budgetRepository.UpdateAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}