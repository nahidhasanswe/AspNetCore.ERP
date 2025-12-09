using ERP.Core;
using ERP.Finance.Application.Budgeting.DTOs;
using ERP.Finance.Domain.Shared.Enums;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Queries.ListBudgets;

public class ListBudgetsQuery : IRequest<Result<IEnumerable<BudgetSummaryDto>>>
{
    public BudgetStatus? Status { get; set; }
    public string? FiscalPeriod { get; set; }
    public Guid? BusinessUnitId { get; set; }
}