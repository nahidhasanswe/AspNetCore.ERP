using ERP.Core;
using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.FiscalYear.Enums;

namespace ERP.Finance.Domain.FiscalYear.Aggregates;

public class FiscalPeriod : AggregateRoot
{
    public string Name { get; private set; } // e.g., "2026-01"
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public PeriodStatus Status { get; private set; }

    private FiscalPeriod() {}

    public FiscalPeriod(string name, DateTime startDate, DateTime endDate)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Status = PeriodStatus.Open;
    }

    public Result Close(PeriodStatus newStatus) 
    {
        if (newStatus == PeriodStatus.Open) 
            throw new DomainException("Cannot close to Open status.");
        if (Status == PeriodStatus.HardClose) 
            throw new DomainException("Cannot modify a hard-closed period.");
        
        Status = newStatus;
        // Optionally raise a Domain Event: PeriodStatusChangedEvent
        
        return Result.Success();
    }
    
    public Result Reopen()
    {
        if (Status == PeriodStatus.HardClose) 
            throw new DomainException("Hard-closed periods cannot be reopened.");
        Status = PeriodStatus.Open;
        
        return Result.Success();
    }
}