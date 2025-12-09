using ERP.Core;
using ERP.Finance.Application.Budgeting.DTOs;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Queries.GetBudgetAvailabilityReport;

public class GetBudgetAvailabilityReportQueryHandler(
    IBudgetRepository budgetRepository,
    IAccountRepository accountRepository,
    IGLReportingService glReportingService)
    : IRequestHandler<GetBudgetAvailabilityReportQuery, Result<IEnumerable<BudgetAvailabilityDto>>>
{
    public async Task<Result<IEnumerable<BudgetAvailabilityDto>>> Handle(GetBudgetAvailabilityReportQuery request, CancellationToken cancellationToken)
    {
        var budget = await budgetRepository.GetByIdAsync(request.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure<IEnumerable<BudgetAvailabilityDto>>("Budget not found.");
        }

        var reportLines = new List<BudgetAvailabilityDto>();
        var budgetName = budget.Name;

        foreach (var item in budget.Items)
        {
            var accountName = await accountRepository.GetAccountNameAsync(item.AccountId, cancellationToken);
            var actualAmountMoney = await glReportingService.GetActualAmountForAccountAndPeriod(
                item.AccountId,
                item.Period,
                item.CostCenterId,
                item.BudgetedAmount.Currency
            );

            var budgetedAmount = item.BudgetedAmount.Amount;
            var committedAmount = item.CommittedAmount.Amount;
            var actualAmount = actualAmountMoney.Amount;

            var availableToCommit = budgetedAmount - committedAmount;
            var availableToSpend = budgetedAmount - committedAmount - actualAmount;

            reportLines.Add(new BudgetAvailabilityDto(
                budget.Id,
                budgetName,
                item.AccountId,
                accountName ?? "Unknown Account",
                item.Period,
                budgetedAmount,
                committedAmount,
                availableToCommit,
                availableToSpend
            ));
        }

        return Result.Success(reportLines.AsEnumerable());
    }
}