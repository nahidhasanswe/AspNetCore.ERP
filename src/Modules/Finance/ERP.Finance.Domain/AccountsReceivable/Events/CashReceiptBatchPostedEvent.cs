using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record CashReceiptBatchPostedEvent(
    Guid BatchId,
    DateTime BatchDate,
    Guid CashAccountId,
    Money TotalBatchAmount,
    string Reference,
    IReadOnlyCollection<Guid> ReceiptIds
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}