using ERP.Core.Events;
using ERP.Finance.Domain.Budgeting.Aggregates;

namespace ERP.Finance.Domain.Budgeting.Events;

public record BudgetApprovedEvent(
    Guid BudgetId,
    Guid BusinessUnitId,
    string FiscalPeriod,
    IReadOnlyCollection<BudgetItem> LineItems
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}