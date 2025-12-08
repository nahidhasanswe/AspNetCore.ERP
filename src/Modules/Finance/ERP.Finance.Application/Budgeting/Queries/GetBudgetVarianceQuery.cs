using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Domain.Budgeting.DTOs;

namespace ERP.Finance.Application.Budgeting.Queries;

public class GetBudgetVarianceQuery : IQuery<Result<BudgetComparisonResult>>
{
    public Guid BudgetId { get; set; }
    public string Period { get; set; }
}