using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record CustomerCreditMemoIssuedEvent(
    Guid CreditMemoId,
    Guid CustomerId,
    Guid BusinessUnitId, // New property
    Money Amount,
    DateTime IssueDate,
    Guid ARControlAccountId,
    Guid RevenueAdjustmentAccountId, // GL account to debit (e.g., Sales Returns)
    string Reason
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}