using ERP.Core;
using MediatR;

namespace ERP.Finance.Application.Budgeting.Commands.UpdateBudget;

public class UpdateBudgetCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; } // New property
    public Guid BudgetId { get; set; }
    public string Name { get; set; }
    public string FiscalPeriod { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? ParentBudgetId { get; set; } // Added for hierarchy
}