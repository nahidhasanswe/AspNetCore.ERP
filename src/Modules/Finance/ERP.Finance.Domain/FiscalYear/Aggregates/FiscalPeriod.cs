using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.FiscalYear.Enums;
using ERP.Finance.Domain.FiscalYear.Events;

namespace ERP.Finance.Domain.FiscalYear.Aggregates;

public class FiscalPeriod : AggregateRoot
{
    public string Name { get; private set; } // e.g., "2026-01"
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public PeriodStatus Status { get; private set; }

    private FiscalPeriod() { }

    // Updated constructor to accept FiscalYearId
    public FiscalPeriod(string name, DateTime startDate, DateTime endDate) : base(Guid.NewGuid())
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Status = PeriodStatus.NeverOpened;
    }

    public void Update(string newName, DateTime newStartDate, DateTime newEndDate)
    {
        if (Status != PeriodStatus.NeverOpened && Status != PeriodStatus.Open)
            throw new DomainException("Only periods that are NeverOpened or Open can be updated.");
        if (newStartDate >= newEndDate)
            throw new DomainException("Start date must be before end date.");

        Name = newName;
        StartDate = newStartDate;
        EndDate = newEndDate;
    }

    public void Open()
    {
        if (Status != PeriodStatus.NeverOpened)
            throw new DomainException("Period has already been opened or closed.");
        Status = PeriodStatus.Open;
        AddDomainEvent(new FiscalPeriodOpenedEvent(this.Id, this.Name, this.StartDate, this.EndDate));
    }

    public void SoftClose()
    {
        if (Status != PeriodStatus.Open)
            throw new DomainException("Can only soft-close an open period.");
        Status = PeriodStatus.SoftClose;
        AddDomainEvent(new FiscalPeriodSoftClosedEvent(this.Id, this.Name, this.StartDate, this.EndDate));
    }

    public void HardClose()
    {
        if (Status != PeriodStatus.SoftClose)
            throw new DomainException("Can only hard-close a soft-closed period.");
        Status = PeriodStatus.HardClose;
        AddDomainEvent(new FiscalPeriodHardClosedEvent(this.Id, this.Name, this.StartDate, this.EndDate));
    }

    public void Reopen()
    {
        if (Status == PeriodStatus.HardClose) 
            throw new DomainException("Hard-closed periods cannot be reopened.");
        Status = PeriodStatus.Open;
        AddDomainEvent(new FiscalPeriodReopenedEvent(this.Id, this.Name, this.StartDate, this.EndDate));
    }

    public void PostClosingEntry()
    {
        if (Status != PeriodStatus.SoftClose)
        {
            throw new DomainException("A closing entry can only be posted to a soft-closed period.");
        }
        // This method conceptually allows a 'closing' post and immediately hard-closes the period.
        Status = PeriodStatus.HardClose;
        AddDomainEvent(new FiscalPeriodHardClosedEvent(this.Id, this.Name, this.StartDate, this.EndDate));
    }
}