using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record PaymentUnappliedEvent(
    Guid InvoiceId,
    Money AmountUnapplied,
    DateTime UnappliedDate,
    Guid ARControlAccountId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}