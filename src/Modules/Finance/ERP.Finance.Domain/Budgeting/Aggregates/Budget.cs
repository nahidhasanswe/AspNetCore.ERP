using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.Budgeting.Events;
using ERP.Finance.Domain.Shared.Enums;

namespace ERP.Finance.Domain.Budgeting.Aggregates;

public class Budget : AggregateRoot
{
    public string Name { get; private set; }
    public string FiscalPeriod { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public BudgetStatus Status { get; private set; }
    public Guid BusinessUnitId { get; private set; }

    private readonly List<BudgetItem> _items = new();
    public IReadOnlyCollection<BudgetItem> Items => _items.AsReadOnly();

    private Budget() { }

    public Budget(Guid businessUnitId, string name, string fiscalYear, DateTime startDate, DateTime endDate) : base(Guid.NewGuid())
    {
        if (startDate >= endDate) throw new DomainException("Start date must be before end date.");
        
        Name = name;
        FiscalPeriod = fiscalYear;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        Status = BudgetStatus.Draft;
        BusinessUnitId = businessUnitId;
    }
    
    public void AddItem(BudgetItem item)
    {
        if (Status != BudgetStatus.Draft)
            throw new InvalidOperationException("Only draft budgets can be modified.");
        
        // Invariant: Check for duplicate GL Account and Period combination
        if (_items.Any(i => i.AccountId == item.AccountId && i.Period == item.Period))
            throw new DomainException("A budget item already exists for this account and period.");
            
        _items.Add(item);
    }
    
    public void Approve()
    {
        if (Status != BudgetStatus.Draft && Status != BudgetStatus.PendingApproval)
            throw new InvalidOperationException($"Budget cannot be approved from current state {Status}.");
        
        AddDomainEvent(new BudgetApprovedEvent(this.Id, this.BusinessUnitId, this.FiscalPeriod, this.Items));
        
        Status = BudgetStatus.Approved;
    }
}