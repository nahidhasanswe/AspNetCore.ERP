using ERP.Core;
using ERP.Finance.Application.Budgeting.DTOs;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Domain.GeneralLedger.Aggregates; // For IGLAccountRepository
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.Budgeting.Queries.GetBudgetById;

public class GetBudgetByIdQueryHandler(IBudgetRepository budgetRepository, IAccountRepository accountRepository)
    : IRequestHandler<GetBudgetByIdQuery, Result<BudgetDetailsDto>>
{
    public async Task<Result<BudgetDetailsDto>> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var budget = await budgetRepository.GetByIdAsync(request.BudgetId, cancellationToken);
        if (budget == null)
        {
            return Result.Failure<BudgetDetailsDto>("Budget not found.");
        }

        var itemDetailsTasks = budget.Items.Select(async item =>
        {
            var accountName = await accountRepository.GetAccountNameAsync(item.AccountId, cancellationToken);
            return new BudgetItemDetailsDto(
                item.Id,
                item.AccountId,
                accountName ?? "Unknown Account",
                item.BudgetedAmount,
                item.CommittedAmount,
                item.Period,
                item.CostCenterId
            );
        }).ToList();

        var itemDetails = await Task.WhenAll(itemDetailsTasks);

        var budgetDetails = new BudgetDetailsDto(
            budget.Id,
            budget.BusinessUnitId,
            budget.Name,
            budget.FiscalPeriod,
            budget.StartDate,
            budget.EndDate,
            budget.Status,
            itemDetails
        );

        return Result.Success(budgetDetails);
    }
}