using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using ERP.Finance.Domain.AccountsPayable.Events;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record InvoiceCancelledEvent(
    Guid InvoiceId,
    Guid BusinessUnitId, // New property
    Money TotalAmount,
    Guid ARControlAccountId,
    IEnumerable<CustomerInvoiceLineItemProjection> LineItems,
    string Reason,
    DateTime CancellationDate,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
