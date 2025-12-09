using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Service;
using ERP.Finance.Domain.Encumbrance.Aggregates;
using MediatR;
using ERP.Finance.Domain.Budgeting.Aggregates;

namespace ERP.Finance.Application.Encumbrance.Commands.ReleaseEncumbrance;

public class ReleaseEncumbranceCommandHandler(
    IEncumbranceRepository encumbranceRepository,
    IBudgetRepository budgetRepository,
    IBudgetService budgetService,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ReleaseEncumbranceCommand, Result>
{
    public async Task<Result> Handle(ReleaseEncumbranceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var encumbrance = await encumbranceRepository.GetByIdAsync(command.EncumbranceId, cancellationToken);
        if (encumbrance == null)
        {
            return Result.Failure("Encumbrance not found.");
        }

        encumbrance.Release(); // Update encumbrance status

        // Release funds in the budget
        var releaseResult = await budgetService.ReleaseFunds(
            command.BusinessUnitId,
            command.GlAccountId,
            command.BudgetPeriod,
            command.CostCenterId,
            command.Amount,
            encumbrance.SourceTransactionId // Use the encumbrance's source transaction ID
        );

        if (releaseResult.IsFailure)
        {
            return Result.Failure(releaseResult.Error);
        }

        var updatedBudget = releaseResult.Value;

        await encumbranceRepository.UpdateAsync(encumbrance, cancellationToken);
        await budgetRepository.UpdateAsync(updatedBudget, cancellationToken); // Save the updated budget

        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}