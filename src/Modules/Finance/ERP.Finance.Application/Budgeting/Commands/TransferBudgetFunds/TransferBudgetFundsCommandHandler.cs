using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.TransferBudgetFunds;

public class TransferBudgetFundsCommandHandler(IBudgetRepository budgetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestHandler<TransferBudgetFundsCommand, Result>
{
    public async Task<Result> Handle(TransferBudgetFundsCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var budget = await budgetRepository.GetByIdAsync(command.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure("Budget not found.");
        }

        var transferResult = budget.TransferFunds(command.FromBudgetItemId, command.ToBudgetItemId, command.Amount);
        if (transferResult.IsFailure)
        {
            return transferResult;
        }

        await budgetRepository.UpdateAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}