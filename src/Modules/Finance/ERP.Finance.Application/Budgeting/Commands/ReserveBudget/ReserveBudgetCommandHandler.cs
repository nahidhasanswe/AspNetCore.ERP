using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Domain.Budgeting.Events;
using ERP.Finance.Domain.Encumbrance.Aggregates;
using ERP.Finance.Domain.Shared.Enums;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.ReserveBudget;

public class ReserveBudgetCommandHandler(
    IBudgetRepository budgetRepository,
    IEncumbranceRepository encumbranceRepository, // New repository for tracking reservations
    IMediator mediator,
    IUnitOfWorkManager unitOfWork
    ) : IRequestHandler<ReserveBudgetCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ReserveBudgetCommand command, CancellationToken cancellationToken)
    {
        // 1. Locate the active budget for the period/BU
        var budget = await budgetRepository.GetActiveBudgetForAccountAsync(command.GlAccountId, command.CostCenterId);
        if (budget == null || budget.Status != BudgetStatus.Approved)
            return Result.Failure<Guid>("No active budget found for this account/period.");

        // 2. Locate the specific budget line item
        var lineItem = budget.Items.FirstOrDefault(li => 
            li.AccountId == command.GlAccountId && li.CostCenterId == command.CostCenterId);
            
        if (lineItem == null) 
            return Result.Failure<Guid>("Budget line item not defined.");

        // 3. Execute Domain Logic: Reserve funds
        var reserveResult = lineItem.Reserve(command.Amount);

        if (reserveResult.IsFailure)
        {
            // 4. Dispatch rejection event
            await mediator.Publish(new BudgetCheckedEvent(command.SourceTransactionId, false, reserveResult.Error), cancellationToken);
            return Result.Failure<Guid>(reserveResult.Error);
        }

        // 5. Create a new Encumbrance record (an aggregate or entity)
        var encumbrance = new Domain.Encumbrance.Aggregates.Encumbrance(command.SourceTransactionId, command.Amount, command.GlAccountId, command.CostCenterId);

        // 6. Persist and Dispatch Approval Event (BudgetCheckedEvent)
        using var scope = unitOfWork.Begin();
        await budgetRepository.UpdateAsync(budget, cancellationToken);
        await encumbranceRepository.AddAsync(encumbrance, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
        
        // 7. Dispatch success event
        await mediator.Publish(new BudgetCheckedEvent(command.SourceTransactionId, true, null), cancellationToken);
        
        return Result.Success(encumbrance.Id);
    }
}