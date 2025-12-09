using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;

namespace ERP.Finance.Application.Budgeting.Commands.CreateBudgetRollForward;

public class CreateBudgetRollForwardCommandHandler(IBudgetRepository budgetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateBudgetRollForwardCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateBudgetRollForwardCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var sourceBudget = await budgetRepository.GetByIdAsync(command.SourceBudgetId, cancellationToken);
        if (sourceBudget == null)
        {
            return Result.Failure<Guid>("Source Budget not found.");
        }

        var newBudget = sourceBudget.RollForward(
            command.NewFiscalPeriod,
            command.NewStartDate,
            command.NewEndDate,
            command.AdjustmentFactor
        );

        await budgetRepository.AddAsync(newBudget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(newBudget.Id);
    }
}