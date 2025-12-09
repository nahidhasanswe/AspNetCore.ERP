using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Service;
using ERP.Finance.Domain.Encumbrance.Aggregates;
using MediatR;
using ERP.Finance.Domain.Budgeting.Aggregates;

namespace ERP.Finance.Application.Encumbrance.Commands.LiquidateEncumbrance;

public class LiquidateEncumbranceCommandHandler(
    IEncumbranceRepository encumbranceRepository,
    IBudgetRepository budgetRepository,
    IBudgetService budgetService,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<LiquidateEncumbranceCommand, Result>
{
    public async Task<Result> Handle(LiquidateEncumbranceCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var encumbrance = await encumbranceRepository.GetByIdAsync(command.EncumbranceId, cancellationToken);
        if (encumbrance == null)
        {
            return Result.Failure("Encumbrance not found.");
        }

        encumbrance.Liquidate(command.ActualTransactionId); // Update encumbrance status

        // Liquidate funds in the budget
        var liquidateResult = await budgetService.LiquidateFunds(
            command.BusinessUnitId,
            command.GlAccountId,
            command.BudgetPeriod,
            command.CostCenterId,
            command.Amount,
            encumbrance.SourceTransactionId // Use the encumbrance's source transaction ID
        );

        if (liquidateResult.IsFailure)
        {
            return Result.Failure(liquidateResult.Error);
        }

        var updatedBudget = liquidateResult.Value;

        await encumbranceRepository.UpdateAsync(encumbrance, cancellationToken);
        await budgetRepository.UpdateAsync(updatedBudget, cancellationToken); // Save the updated budget

        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}