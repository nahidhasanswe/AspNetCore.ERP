using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record CashReceiptCancelledEvent(
    Guid ReceiptId,
    Guid CustomerId,
    Money Amount,
    Guid CashAccountId,
    string TransactionReference,
    DateTime CancellationDate
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}