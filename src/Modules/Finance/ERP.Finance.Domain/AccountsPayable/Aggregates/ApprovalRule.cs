using ERP.Core.Entities;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public class ApprovalRule : Entity
{
    public Money AmountThreshold { get; private set; }
    public Guid RequiredApproverId { get; private set; } // Represents a user or role

    private ApprovalRule() { }

    public ApprovalRule(Money amountThreshold, Guid requiredApproverId) : base(Guid.NewGuid())
    {
        AmountThreshold = amountThreshold;
        RequiredApproverId = requiredApproverId;
    }
}