using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.Budgeting.Commands.CreateBudgetRevision;

public class CreateBudgetRevisionCommandHandler(IBudgetRepository budgetRepository, IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateBudgetRevisionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateBudgetRevisionCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var originalBudget = await budgetRepository.GetByIdAsync(command.OriginalBudgetId, cancellationToken);
        if (originalBudget == null)
        {
            return Result.Failure<Guid>("Original Budget not found.");
        }

        var newBudgetRevision = originalBudget.CreateRevision(command.NewNameSuffix);

        await budgetRepository.AddAsync(newBudgetRevision, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(newBudgetRevision.Id);
    }
}