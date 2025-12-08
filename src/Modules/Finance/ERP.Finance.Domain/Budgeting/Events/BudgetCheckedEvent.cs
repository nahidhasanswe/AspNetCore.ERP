using ERP.Core.Events;

namespace ERP.Finance.Domain.Budgeting.Events;

public record BudgetCheckedEvent(
    Guid SourceTransactionId, // Purchase Requisition ID
    bool IsApproved,
    string? ReasonForRejection
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}