using ERP.Core.Entities;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public class ApprovalRule : Entity
{
    public Guid BusinessUnitId { get; private set; }
    public Money AmountThreshold { get; private set; }
    public Guid RequiredApproverId { get; private set; } // Represents a user or role

    private ApprovalRule() { }

    public ApprovalRule(Guid businessUnitId, Money amountThreshold, Guid requiredApproverId) : base(Guid.NewGuid())
    {
        BusinessUnitId = businessUnitId;
        AmountThreshold = amountThreshold;
        RequiredApproverId = requiredApproverId;
    }
}