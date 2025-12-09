using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.Budgeting.Events;
using ERP.Finance.Domain.Budgeting.Service; // Added for IBudgetApprovalService
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using ERP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added for async Approve method

namespace ERP.Finance.Domain.Budgeting.Aggregates;

public class Budget : AggregateRoot
{
    public string Name { get; private set; }
    public string FiscalPeriod { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public BudgetStatus Status { get; private set; }
    public Guid BusinessUnitId { get; private set; }
    public Guid? ParentBudgetId { get; private set; } // New property for hierarchy

    private readonly List<BudgetItem> _items = new();
    public IReadOnlyCollection<BudgetItem> Items => _items.AsReadOnly();

    private readonly List<Guid> _approverIds = new(); // For advanced approval workflow
    public IReadOnlyCollection<Guid> ApproverIds => _approverIds.AsReadOnly();

    private Budget() { }

    public Budget(Guid businessUnitId, string name, string fiscalYear, DateTime startDate, DateTime endDate, Guid? parentBudgetId = null) : base(Guid.NewGuid())
    {
        if (startDate >= endDate) throw new DomainException("Start date must be before end date.");
        
        Name = name;
        FiscalPeriod = fiscalYear;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        Status = BudgetStatus.Draft;
        BusinessUnitId = businessUnitId;
        ParentBudgetId = parentBudgetId;
    }

    // Private constructor for creating revisions or roll-forwards
    private Budget(Guid businessUnitId, string name, string fiscalPeriod, DateTime startDate, DateTime endDate, IEnumerable<BudgetItem> items, Guid? parentBudgetId = null) : base(Guid.NewGuid())
    {
        Name = name;
        FiscalPeriod = fiscalPeriod;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        Status = BudgetStatus.Draft; // Revisions/Roll-forwards start as Draft
        BusinessUnitId = businessUnitId;
        ParentBudgetId = parentBudgetId;
        _items.AddRange(items.Select(item => new BudgetItem(item.AccountId, item.BudgetedAmount, item.Period, item.CostCenterId)));
    }

    public Budget CreateRevision(string newNameSuffix = " - Revision")
    {
        if (Status != BudgetStatus.Approved)
            throw new InvalidOperationException("Only approved budgets can be revised.");

        // Create new budget items based on current items
        var revisedItems = Items.Select(item => new BudgetItem(item.AccountId, item.BudgetedAmount, item.Period, item.CostCenterId)).ToList();

        return new Budget(
            BusinessUnitId,
            Name + newNameSuffix,
            FiscalPeriod,
            StartDate,
            EndDate,
            revisedItems,
            ParentBudgetId // Maintain parent relationship in revision
        );
    }

    public Budget RollForward(string newFiscalPeriod, DateTime newStartDate, DateTime newEndDate, decimal adjustmentFactor = 1.0m)
    {
        // Can roll forward from Approved or Archived budgets
        if (Status != BudgetStatus.Approved && Status != BudgetStatus.Archived)
            throw new InvalidOperationException("Only approved or archived budgets can be rolled forward.");
        if (newStartDate >= newEndDate) throw new DomainException("New start date must be before new end date.");

        var rolledForwardItems = Items.Select(item => new BudgetItem(
            item.AccountId,
            new Money(item.BudgetedAmount.Amount * adjustmentFactor, item.BudgetedAmount.Currency),
            newFiscalPeriod, // Period should reflect the new fiscal period
            item.CostCenterId
        )).ToList();

        return new Budget(
            BusinessUnitId,
            $"{Name} - {newFiscalPeriod}", // New name
            newFiscalPeriod,
            newStartDate,
            newEndDate,
            rolledForwardItems,
            ParentBudgetId // Maintain parent relationship in roll-forward
        );
    }

    public void Update(string name, string fiscalPeriod, DateTime startDate, DateTime endDate, Guid? parentBudgetId)
    {
        if (Status != BudgetStatus.Draft)
            throw new InvalidOperationException("Only draft budgets can be updated.");
        if (startDate >= endDate) throw new DomainException("Start date must be before end date.");

        Name = name;
        FiscalPeriod = fiscalPeriod;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        ParentBudgetId = parentBudgetId;
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

    public void UpdateItem(Guid budgetItemId, Money newBudgetedAmount, string newPeriod, Guid? newCostCenterId)
    {
        if (Status != BudgetStatus.Draft)
            throw new InvalidOperationException("Only draft budgets can be modified.");

        var itemToUpdate = _items.FirstOrDefault(item => item.Id == budgetItemId);
        if (itemToUpdate == null)
            throw new DomainException($"Budget item with ID {budgetItemId} not found.");

        itemToUpdate.Update(newBudgetedAmount, newPeriod, newCostCenterId);
    }

    public void RemoveItem(Guid budgetItemId)
    {
        if (Status != BudgetStatus.Draft)
            throw new InvalidOperationException("Only draft budgets can be modified.");

        var itemToRemove = _items.FirstOrDefault(item => item.Id == budgetItemId);
        if (itemToRemove == null)
            throw new DomainException($"Budget item with ID {budgetItemId} not found.");

        _items.Remove(itemToRemove);
    }

    public void SubmitForApproval()
    {
        if (Status != BudgetStatus.Draft)
            throw new InvalidOperationException("Only draft budgets can be submitted for approval.");
        if (!_items.Any())
            throw new DomainException("Cannot submit an empty budget for approval.");
        
        Status = BudgetStatus.PendingApproval;
    }
    
    public async Task Approve(Guid approverId, IBudgetApprovalService approvalService) // Modified to be async and take service
    {
        if (Status != BudgetStatus.Draft && Status != BudgetStatus.PendingApproval)
            throw new InvalidOperationException($"Budget cannot be approved from current state {Status}.");

        if (!_approverIds.Contains(approverId))
        {
            _approverIds.Add(approverId);
        }

        if (await approvalService.HasSufficientApproval(this, approverId))
        {
            Status = BudgetStatus.Approved;
            AddDomainEvent(new BudgetApprovedEvent(this.Id, this.BusinessUnitId, this.FiscalPeriod, this.Items));
        }
        else
        {
            Status = BudgetStatus.PendingApproval; // Remain in pending if not fully approved
        }
    }

    public void Reject()
    {
        if (Status != BudgetStatus.PendingApproval)
            throw new InvalidOperationException("Only budgets pending approval can be rejected.");
        
        Status = BudgetStatus.Draft;
        // Optionally, add a domain event for BudgetRejectedEvent
    }

    public void Activate()
    {
        if (Status != BudgetStatus.Approved)
            throw new InvalidOperationException("Only approved budgets can be activated.");
        // If we had a separate 'Active' status, we'd set it here.
        // For now, 'Approved' implies active. This method could be used for future extensions.
    }

    public void Deactivate()
    {
        if (Status != BudgetStatus.Approved) // Only approved budgets can be deactivated
            throw new InvalidOperationException("Only approved budgets can be deactivated.");
        
        Status = BudgetStatus.Archived; // Or a new 'Inactive' status
    }

    public void Close()
    {
        if (Status == BudgetStatus.Archived)
            throw new InvalidOperationException("Budget is already closed/archived.");
        
        Status = BudgetStatus.Archived; // Or a new 'Closed' status
        // Add domain event for BudgetClosedEvent if needed
    }

    public Result ReleaseFunds(Guid budgetItemId, Money amount)
    {
        var item = _items.FirstOrDefault(i => i.Id == budgetItemId);
        if (item == null) return Result.Failure($"Budget item with ID {budgetItemId} not found.");
        
        return item.Release(amount);
    }

    public Result LiquidateFunds(Guid budgetItemId, Money amount)
    {
        var item = _items.FirstOrDefault(i => i.Id == budgetItemId);
        if (item == null) return Result.Failure($"Budget item with ID {budgetItemId} not found.");

        return item.Liquidate(amount);
    }

    public Result TransferFunds(Guid fromBudgetItemId, Guid toBudgetItemId, Money amount)
    {
        if (Status != BudgetStatus.Draft && Status != BudgetStatus.Approved)
            return Result.Failure("Funds can only be transferred in Draft or Approved budgets.");
        if (amount.Amount <= 0)
            return Result.Failure("Transfer amount must be positive.");
        
        var fromItem = _items.FirstOrDefault(item => item.Id == fromBudgetItemId);
        if (fromItem == null) return Result.Failure($"Source budget item with ID {fromBudgetItemId} not found.");
        
        var toItem = _items.FirstOrDefault(item => item.Id == toBudgetItemId);
        if (toItem == null) return Result.Failure($"Destination budget item with ID {toBudgetItemId} not found.");
        
        if (fromItem.BudgetedAmount.Currency != amount.Currency || toItem.BudgetedAmount.Currency != amount.Currency)
            return Result.Failure("Cannot transfer funds between different currencies.");
        if (fromItem.BudgetedAmount.Currency != toItem.BudgetedAmount.Currency)
            return Result.Failure("Cannot transfer funds between budget items of different currencies.");

        // Check if enough funds are available to transfer from the source
        if (fromItem.BudgetedAmount.Amount - fromItem.CommittedAmount.Amount < amount.Amount)
            return Result.Failure("Insufficient available funds in the source budget item for transfer.");

        // Perform the transfer
        fromItem.AdjustBudgetedAmount(new Money(-amount.Amount, amount.Currency)); // Decrease source
        toItem.AdjustBudgetedAmount(new Money(amount.Amount, amount.Currency));    // Increase destination

        // Optionally, raise a domain event for BudgetFundsTransferredEvent
        return Result.Success();
    }
}