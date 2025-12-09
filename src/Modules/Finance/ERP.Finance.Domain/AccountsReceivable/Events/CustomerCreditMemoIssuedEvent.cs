using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record CustomerCreditMemoIssuedEvent(
    Guid CreditMemoId,
    Guid CustomerId,
    Money Amount,
    DateTime IssueDate,
    Guid ARControlAccountId,
    Guid RevenueAdjustmentAccountId, // GL account to debit (e.g., Sales Returns)
    string Reason
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}