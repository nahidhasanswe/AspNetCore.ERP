using ERP.Core.Entities;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.Budgeting.Aggregates;

public class BudgetApprovalRule : Entity
{
    public Guid BusinessUnitId { get; private set; }
    public Money AmountThreshold { get; private set; }
    public Guid RequiredApproverId { get; private set; } // Represents a user or role

    private BudgetApprovalRule() { }

    public BudgetApprovalRule(Guid businessUnitId, Money amountThreshold, Guid requiredApproverId) : base(Guid.NewGuid())
    {
        BusinessUnitId = businessUnitId;
        AmountThreshold = amountThreshold;
        RequiredApproverId = requiredApproverId;
    }
}