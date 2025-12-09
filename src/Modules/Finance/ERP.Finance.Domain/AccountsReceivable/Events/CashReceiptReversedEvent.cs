using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record CashReceiptReversedEvent(
    Guid ReceiptId,
    Guid CustomerId,
    Money TotalReceivedAmount,
    Guid CashAccountId,
    string TransactionReference,
    string Reason,
    DateTime ReversalDate
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}