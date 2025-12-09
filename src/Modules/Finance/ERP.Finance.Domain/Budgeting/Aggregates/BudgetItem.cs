using ERP.Core;
using ERP.Core.Entities;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.Budgeting.Aggregates;

public class BudgetItem : Entity
{
    public Guid AccountId { get; private set; } // GL Account ID
    public Money BudgetedAmount { get; private set; } // The planned spending/revenue
    public Money CommittedAmount { get; private set; } = new Money(0m, "USD");
    public string Period { get; private set; } // e.g., "JAN-2026", "Q1-2026"
    public Guid? CostCenterId { get; private set; }

    private BudgetItem() { }

    public BudgetItem(Guid accountId, Money budgetedAmount, string period, Guid? costCenterId) : base(Guid.NewGuid())
    {
        if (budgetedAmount.Amount <= 0) throw new ArgumentException("Budgeted amount must be positive.");
        
        AccountId = accountId;
        BudgetedAmount = budgetedAmount;
        Period = period;
        CostCenterId = costCenterId;
    }

    public BudgetItem(Guid id, Guid accountId, Money budgetedAmount, string period, Guid? costCenterId) : base(id)
    {
        if (budgetedAmount.Amount <= 0) throw new ArgumentException("Budgeted amount must be positive.");

        AccountId = accountId;
        BudgetedAmount = budgetedAmount;
        Period = period;
        CostCenterId = costCenterId;
    }

    public void Update(Money newBudgetedAmount, string newPeriod, Guid? newCostCenterId)
    {
        if (newBudgetedAmount.Amount <= 0) throw new ArgumentException("Budgeted amount must be positive.");
        // Add checks to ensure newBudgetedAmount is not less than CommittedAmount if desired
        
        BudgetedAmount = newBudgetedAmount;
        Period = newPeriod;
        CostCenterId = newCostCenterId;
    }
    
    public Result Reserve(Money amount)
    {
        if (amount.Amount > (BudgetedAmount.Amount - CommittedAmount.Amount))
            return Result.Failure("Budget exhausted.");

        CommittedAmount = new Money(CommittedAmount.Amount + amount.Amount, amount.Currency);
        return Result.Success();
    }

    public Result Release(Money amount)
    {
        if (amount.Amount <= 0) return Result.Failure("Release amount must be positive.");
        if (amount.Amount > CommittedAmount.Amount) return Result.Failure("Cannot release more than committed amount.");

        CommittedAmount = new Money(CommittedAmount.Amount - amount.Amount, amount.Currency);
        return Result.Success();
    }

    public Result Liquidate(Money amount)
    {
        if (amount.Amount <= 0) return Result.Failure("Liquidate amount must be positive.");
        if (amount.Amount > CommittedAmount.Amount) return Result.Failure("Cannot liquidate more than committed amount.");

        CommittedAmount = new Money(CommittedAmount.Amount - amount.Amount, amount.Currency);
        return Result.Success();
    }
    
    // Note: We do NOT track Actuals here. Actuals are read from the GL context.
}