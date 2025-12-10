using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.UpdateBudget;

public class UpdateBudgetCommandHandler(IBudgetRepository budgetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateBudgetCommand, Result>
{
    public async Task<Result> Handle(UpdateBudgetCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var budget = await budgetRepository.GetByIdAsync(command.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure("Budget not found.");
        }

        // Add validation for BusinessUnitId
        if (budget.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Budget does not belong to the specified Business Unit.");
        }

        budget.Update(command.Name, command.FiscalPeriod, command.StartDate, command.EndDate, command.ParentBudgetId);

        await budgetRepository.UpdateAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}