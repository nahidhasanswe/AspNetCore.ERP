using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Domain.Budgeting.Service;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.ApproveBudget;

public class ApproveBudgetCommandHandler(
    IBudgetRepository budgetRepository,
    IBudgetApprovalService budgetApprovalService,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ApproveBudgetCommand, Result>
{
    public async Task<Result> Handle(ApproveBudgetCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var budget = await budgetRepository.GetByIdAsync(command.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure("Budget not found.");
        }

        await budget.Approve(command.ApproverId, budgetApprovalService);

        await budgetRepository.UpdateAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}