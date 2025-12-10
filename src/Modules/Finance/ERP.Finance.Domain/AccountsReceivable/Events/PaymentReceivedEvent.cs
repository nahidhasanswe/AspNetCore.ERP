using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record PaymentReceivedEvent(
    Guid InvoiceId,
    Guid BusinessUnitId, // New property
    Money AmountReceived,
    DateTime PaymentDate,
    string Reference,
    Guid ArControlAccountId,
    Guid CashAccountId,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}