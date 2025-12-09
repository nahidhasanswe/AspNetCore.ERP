using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record CustomerCreditAppliedToInvoiceEvent(
    Guid InvoiceId,
    Guid CreditMemoId,
    Money AmountApplied,
    DateTime AppliedDate,
    Guid ARControlAccountId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}