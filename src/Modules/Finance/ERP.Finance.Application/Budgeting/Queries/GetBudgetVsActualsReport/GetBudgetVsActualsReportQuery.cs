using ERP.Core;
using ERP.Finance.Application.Budgeting.DTOs;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Queries.GetBudgetVsActualsReport;

public class GetBudgetVsActualsReportQuery : IRequest<Result<IEnumerable<BudgetVsActualsDto>>>
{
    public Guid BudgetId { get; set; }
}