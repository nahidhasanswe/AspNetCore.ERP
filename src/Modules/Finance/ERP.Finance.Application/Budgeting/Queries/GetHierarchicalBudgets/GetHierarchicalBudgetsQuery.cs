using ERP.Core;
using ERP.Finance.Application.Budgeting.DTOs;
using MediatR;
using System.Collections.Generic;

namespace ERP.Finance.Application.Budgeting.Queries.GetHierarchicalBudgets;

public class GetHierarchicalBudgetsQuery : IRequest<Result<IEnumerable<HierarchicalBudgetDto>>>
{
    // Optional: Filter by BusinessUnitId or FiscalPeriod
    public Guid? BusinessUnitId { get; set; }
    public string? FiscalPeriod { get; set; }
}