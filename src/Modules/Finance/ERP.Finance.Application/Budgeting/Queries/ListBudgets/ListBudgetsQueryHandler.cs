using ERP.Core;
using ERP.Finance.Application.Budgeting.DTOs;
using ERP.Finance.Domain.Budgeting.Aggregates;
using MediatR;
using ERP.Finance.Domain.Budgeting.Specifications;

namespace ERP.Finance.Application.Budgeting.Queries.ListBudgets;

public class ListBudgetsQueryHandler(IBudgetRepository budgetRepository)
    : IRequestHandler<ListBudgetsQuery, Result<IEnumerable<BudgetSummaryDto>>>
{
    public async Task<Result<IEnumerable<BudgetSummaryDto>>> Handle(ListBudgetsQuery request, CancellationToken cancellationToken)
    {
        var filteredBudgets = await budgetRepository.ListAsync(new GetBudgetSpecifications(request.BusinessUnitId, request.FiscalPeriod, request.Status), cancellationToken);

        var summaryDtos = filteredBudgets.Select(b => new BudgetSummaryDto(
            b.Id,
            b.Name,
            b.FiscalPeriod,
            b.StartDate,
            b.EndDate,
            b.Status,
            b.BusinessUnitId
        )).ToList();

        return Result.Success(summaryDtos.AsEnumerable());
    }
}