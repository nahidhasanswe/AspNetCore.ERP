using ERP.Core;
using ERP.Finance.Application.Budgeting.DTOs;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Services;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Queries.GetBudgetVsActualsReport;

public class GetBudgetVsActualsReportQueryHandler(
    IBudgetRepository budgetRepository,
    IAccountRepository glAccountRepository,
    IGLReportingService glReportingService)
    : IRequestHandler<GetBudgetVsActualsReportQuery, Result<IEnumerable<BudgetVsActualsDto>>>
{
    public async Task<Result<IEnumerable<BudgetVsActualsDto>>> Handle(GetBudgetVsActualsReportQuery request, CancellationToken cancellationToken)
    {
        var budget = await budgetRepository.GetByIdAsync(request.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure<IEnumerable<BudgetVsActualsDto>>("Budget not found.");
        }

        var reportLines = new List<BudgetVsActualsDto>();
        var budgetName = budget.Name;

        foreach (var item in budget.Items)
        {
            var accountName = await glAccountRepository.GetAccountNameAsync(item.AccountId, cancellationToken);
            var actualAmountMoney = await glReportingService.GetActualAmountForAccountAndPeriod(
                item.AccountId,
                item.Period,
                item.CostCenterId,
                item.BudgetedAmount.Currency
            );

            var budgetedAmount = item.BudgetedAmount.Amount;
            var committedAmount = item.CommittedAmount.Amount;
            var actualAmount = actualAmountMoney.Amount;
            var availableAmount = budgetedAmount - committedAmount - actualAmount;
            var variance = budgetedAmount - actualAmount;

            reportLines.Add(new BudgetVsActualsDto(
                budget.Id,
                budgetName,
                item.AccountId,
                accountName ?? "Unknown Account",
                item.Period,
                budgetedAmount,
                committedAmount,
                actualAmount,
                availableAmount,
                variance
            ));
        }

        return Result.Success(reportLines.AsEnumerable());
    }
}