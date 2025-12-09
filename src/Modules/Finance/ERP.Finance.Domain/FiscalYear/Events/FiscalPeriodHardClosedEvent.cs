using ERP.Core.Events;
using System;

namespace ERP.Finance.Domain.FiscalYear.Events;

public record FiscalPeriodHardClosedEvent(
    Guid FiscalPeriodId,
    string PeriodName,
    DateTime StartDate,
    DateTime EndDate
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}