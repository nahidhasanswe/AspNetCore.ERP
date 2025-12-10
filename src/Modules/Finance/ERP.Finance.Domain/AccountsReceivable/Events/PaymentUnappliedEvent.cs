using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record PaymentUnappliedEvent(
    Guid InvoiceId,
    Guid BusinessUnitId, // New property
    Money AmountUnapplied,
    DateTime UnappliedDate,
    Guid ARControlAccountId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}