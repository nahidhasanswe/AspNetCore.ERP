using ERP.Core.Events;

namespace ERP.Finance.Domain.FiscalYear.Events;

public record FiscalPeriodSoftClosedEvent(
    Guid FiscalPeriodId,
    Guid BusinessUnitId, // New property
    string PeriodName,
    DateTime StartDate,
    DateTime EndDate
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}