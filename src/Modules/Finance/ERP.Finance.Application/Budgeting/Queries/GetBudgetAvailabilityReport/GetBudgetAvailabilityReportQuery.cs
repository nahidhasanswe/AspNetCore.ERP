using ERP.Core;
using ERP.Finance.Application.Budgeting.DTOs;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Queries.GetBudgetAvailabilityReport;

public class GetBudgetAvailabilityReportQuery : IRequest<Result<IEnumerable<BudgetAvailabilityDto>>>
{
    public Guid BudgetId { get; set; }
}