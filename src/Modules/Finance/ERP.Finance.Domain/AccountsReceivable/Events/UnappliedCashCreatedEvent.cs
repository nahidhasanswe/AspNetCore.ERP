using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record UnappliedCashCreatedEvent(
    Guid CashReceiptId,
    Guid CustomerId,
    Money TotalReceivedAmount,
    Guid CashAccountId, // The GL account debited
    string TransactionReference
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}