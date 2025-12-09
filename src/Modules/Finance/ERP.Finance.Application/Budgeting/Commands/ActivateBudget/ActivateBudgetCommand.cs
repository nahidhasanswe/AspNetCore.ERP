using ERP.Core.Behaviors;

namespace ERP.Finance.Application.Budgeting.Commands.ActivateBudget;

public class ActivateBudgetCommand : IRequestCommand<Guid>
{
    public Guid BudgetId { get; set; }
}