using ERP.Core;
using ERP.Finance.Application.Budgeting.DTOs;
using ERP.Finance.Domain.Budgeting.Aggregates;
using MediatR;
using ERP.Finance.Domain.Budgeting.Specifications;

namespace ERP.Finance.Application.Budgeting.Queries.GetHierarchicalBudgets;

public class GetHierarchicalBudgetsQueryHandler(IBudgetRepository budgetRepository)
    : IRequestHandler<GetHierarchicalBudgetsQuery, Result<IEnumerable<HierarchicalBudgetDto>>>
{
    public async Task<Result<IEnumerable<HierarchicalBudgetDto>>> Handle(GetHierarchicalBudgetsQuery request, CancellationToken cancellationToken)
    {
        var filteredBudgets = await budgetRepository.ListAsync(new GetBudgetSpecifications(request.BusinessUnitId, request.FiscalPeriod, null) ,cancellationToken);

        var budgetDtos = filteredBudgets.Select(b => new HierarchicalBudgetDto(
            b.Id,
            b.Name,
            b.FiscalPeriod,
            b.Status,
            b.BusinessUnitId,
            b.ParentBudgetId,
            new List<HierarchicalBudgetDto>() // Initialize children list
        )).ToList();

        // Build the hierarchy
        var topLevelBudgets = budgetDtos.Where(b => !b.ParentBudgetId.HasValue).ToList();
        foreach (var topLevelBudget in topLevelBudgets)
        {
            AddChildren(topLevelBudget, budgetDtos);
        }

        return Result.Success(topLevelBudgets.AsEnumerable());
    }

    private void AddChildren(HierarchicalBudgetDto parent, List<HierarchicalBudgetDto> allBudgets)
    {
        var children = allBudgets.Where(b => b.ParentBudgetId == parent.Id).ToList();
        parent.Children.AddRange(children);

        foreach (var child in children)
        {
            AddChildren(child, allBudgets);
        }
    }
}