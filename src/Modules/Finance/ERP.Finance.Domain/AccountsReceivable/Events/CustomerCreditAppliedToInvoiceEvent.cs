using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record CustomerCreditAppliedToInvoiceEvent(
    Guid InvoiceId,
    Guid CreditMemoId,
    Guid BusinessUnitId, // New property
    Money AmountApplied,
    DateTime AppliedDate,
    Guid ARControlAccountId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}