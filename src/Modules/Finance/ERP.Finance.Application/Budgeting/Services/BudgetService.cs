using ERP.Core;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Domain.Budgeting.Service;
using ERP.Finance.Domain.Shared.ValueObjects;
using ERP.Finance.Domain.Budgeting.Specifications;

namespace ERP.Finance.Application.Budgeting.Services;

public class BudgetService(IBudgetRepository budgetRepository) : IBudgetService
{
    // Modified to return the Budget aggregate
    private async Task<Budget?> GetActiveBudget(Guid businessUnitId, string period)
    {
        return await budgetRepository.FirstOrDefaultAsync(
            new GetBudgetSpecifications(businessUnitId, period, Domain.Shared.Enums.BudgetStatus.Approved));
    }

    public async Task<Result> CheckFundsAvailability(Guid businessUnitId, Guid accountId, string period, Guid? costCenterId, Money amount)
    {
        var budget = await GetActiveBudget(businessUnitId, period);
        if (budget is null)
        {
            return Result.Failure("No active budget found for the specified criteria.");
        }

        var budgetItem = budget.Items.FirstOrDefault(item =>
            item.AccountId == accountId &&
            item.Period == period &&
            item.CostCenterId == costCenterId);

        if (budgetItem is null)
        {
            return Result.Failure("No budget item found for the specified criteria.");
        }

        if (amount.Amount > (budgetItem.BudgetedAmount.Amount - budgetItem.CommittedAmount.Amount))
        {
            return Result.Failure("Insufficient funds available in the budget.");
        }

        return Result.Success();
    }

    public async Task<Result<Budget>> ReserveFunds(Guid businessUnitId, Guid accountId, string period, Guid? costCenterId, Money amount, Guid sourceTransactionId)
    {
        var budget = await GetActiveBudget(businessUnitId, period);
        if (budget is null)
        {
            return Result.Failure<Budget>("No active budget found for the specified criteria.");
        }

        var budgetItem = budget.Items.FirstOrDefault(item =>
            item.AccountId == accountId &&
            item.Period == period &&
            item.CostCenterId == costCenterId);

        if (budgetItem is null)
        {
            return Result.Failure<Budget>("No budget item found for the specified criteria.");
        }

        var result = budgetItem.Reserve(amount);
        if (result.IsFailure)
        {
            return Result.Failure<Budget>(result.Error);
        }
        
        return Result.Success(budget); // Return the modified budget
    }

    public async Task<Result<Budget>> ReleaseFunds(Guid businessUnitId, Guid accountId, string period, Guid? costCenterId, Money amount, Guid sourceTransactionId)
    {
        var budget = await GetActiveBudget(businessUnitId, period);
        if (budget is null)
        {
            return Result.Failure<Budget>("No active budget found for the specified criteria.");
        }

        var budgetItem = budget.Items.FirstOrDefault(item =>
            item.AccountId == accountId &&
            item.Period == period &&
            item.CostCenterId == costCenterId);

        if (budgetItem is null)
        {
            return Result.Failure<Budget>("No budget item found for the specified criteria.");
        }

        var result = budgetItem.Release(amount);
        if (result.IsFailure)
        {
            return Result.Failure<Budget>(result.Error);
        }

        return Result.Success(budget); // Return the modified budget
    }

    public async Task<Result<Budget>> LiquidateFunds(Guid businessUnitId, Guid accountId, string period, Guid? costCenterId, Money amount, Guid sourceTransactionId)
    {
        var budget = await GetActiveBudget(businessUnitId, period);
        if (budget is null)
        {
            return Result.Failure<Budget>("No active budget found for the specified criteria.");
        }

        var budgetItem = budget.Items.FirstOrDefault(item =>
            item.AccountId == accountId &&
            item.Period == period &&
            item.CostCenterId == costCenterId);

        if (budgetItem is null)
        {
            return Result.Failure<Budget>("No budget item found for the specified criteria.");
        }

        var result = budgetItem.Liquidate(amount);
        if (result.IsFailure)
        {
            return Result.Failure<Budget>(result.Error);
        }

        return Result.Success(budget); // Return the modified budget
    }
}