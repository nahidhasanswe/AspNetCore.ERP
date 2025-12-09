using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.SubmitBudgetForApproval;

public class SubmitBudgetForApprovalCommandHandler(IBudgetRepository budgetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<SubmitBudgetForApprovalCommand, Result>
{
    public async Task<Result> Handle(SubmitBudgetForApprovalCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var budget = await budgetRepository.GetByIdAsync(command.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure("Budget not found.");
        }

        budget.SubmitForApproval();

        await budgetRepository.UpdateAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}