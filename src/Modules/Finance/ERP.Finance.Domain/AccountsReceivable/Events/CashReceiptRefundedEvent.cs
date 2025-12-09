using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record CashReceiptRefundedEvent(
    Guid ReceiptId,
    Guid CustomerId,
    Money RefundAmount,
    Guid OriginalCashAccountId, // Account where cash was originally received
    Guid RefundCashAccountId,   // Account from which refund is made
    string RefundReference,
    DateTime RefundDate
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}