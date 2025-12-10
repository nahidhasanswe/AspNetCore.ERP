using ERP.Core.Events;

namespace ERP.Finance.Domain.FiscalYear.Events;

public record FiscalPeriodReopenedEvent(
    Guid FiscalPeriodId,
    Guid BusinessUnitId,
    string PeriodName,
    DateTime StartDate,
    DateTime EndDate
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}