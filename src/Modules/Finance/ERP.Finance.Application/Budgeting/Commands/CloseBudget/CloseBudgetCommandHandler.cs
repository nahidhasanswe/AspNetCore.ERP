using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.CloseBudget;

public class CloseBudgetCommandHandler(IBudgetRepository budgetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<CloseBudgetCommand, Result>
{
    public async Task<Result> Handle(CloseBudgetCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var budget = await budgetRepository.GetByIdAsync(command.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure("Budget not found.");
        }

        budget.Close();

        await budgetRepository.UpdateAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}