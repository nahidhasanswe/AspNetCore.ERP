using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Events;

public record VendorInvoiceCancelledEvent(
    Guid InvoiceId,
    Guid VendorId,
    Guid BusinessUnitId, // New property
    string CancellationReason,
    DateTime CancellationDate,
    Money OriginalTotalAmount,
    Guid OriginalApControlAccountId,
    IEnumerable<InvoiceLineItemProjection> OriginalLineItems
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}