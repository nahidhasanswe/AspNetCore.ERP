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

        budget.Update(command.Name, command.FiscalPeriod, command.StartDate, command.EndDate);

        await budgetRepository.UpdateAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}