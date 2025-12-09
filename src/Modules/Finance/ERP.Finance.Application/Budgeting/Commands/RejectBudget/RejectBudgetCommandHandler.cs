using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.RejectBudget;

public class RejectBudgetCommandHandler(IBudgetRepository budgetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<RejectBudgetCommand, Result>
{
    public async Task<Result> Handle(RejectBudgetCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var budget = await budgetRepository.GetByIdAsync(command.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure("Budget not found.");
        }

        budget.Reject();

        await budgetRepository.UpdateAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}