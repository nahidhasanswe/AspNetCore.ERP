using ERP.Core.Exceptions;
using ERP.Finance.Application.GeneralLedger.Queries.GetActual;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Domain.Budgeting.DTOs;
using ERP.Finance.Domain.Budgeting.Service;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Services;

public class BudgetComparisonService(
    IBudgetRepository budgetRepository,
    IMediator mediator,
    IAccountRepository accountRepository
    ) : IBudgetComparisonService
{

    public async Task<List<BudgetComparisonResult>> CompareBudgetToActuals(Guid budgetId, string period, CancellationToken cancellationToken = default)
    {
        var budget = await budgetRepository.GetByIdAsync(budgetId, cancellationToken);
        if (budget == null) throw new NotFoundException($"Budget {budgetId} not found.");

        // For simplicity, we assume the requested period is the whole budget range
        var budgetItems = budget.Items.Where(i => i.Period == period).ToList(); 
        
        var accountIds = budgetItems.Select(i => i.AccountId).Distinct();
        
        // 1. Fetch Actuals from GL context via the ACL interface
        var actualData = await mediator.Send(new GetActualsQuery(accountIds, budget.StartDate, budget.EndDate), cancellationToken);
        
        // 2. Aggregate and Compare
        var results = new List<BudgetComparisonResult>();
        
        foreach (var item in budgetItems)
        {
            // Actual is net of debits/credits for the account
            actualData.Value.TryGetValue(item.AccountId, out decimal actualAmount);
            
            var accountName = await accountRepository.GetAccountNameAsync(item.AccountId, cancellationToken);

            results.Add(new BudgetComparisonResult
            {
                AccountName = accountName,
                Period = item.Period,
                Budgeted = item.BudgetedAmount.Amount,
                Actual = actualAmount // Actual value fetched from GL
            });
        }
        
        return results;
    }
}