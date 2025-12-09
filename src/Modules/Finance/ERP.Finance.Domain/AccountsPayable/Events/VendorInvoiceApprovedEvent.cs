using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Events;

public record VendorInvoiceApprovedEvent(
    Guid InvoiceId,
    Guid VendorId,
    Guid BusinessUnitId, // New property
    Money TotalAmount,
    DateTime ApprovalDate,
    Guid APControlAccountId,
    IEnumerable<InvoiceLineItemProjection> LineItems
)  : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

// Supporting Projection DTO for the Event
public record InvoiceLineItemProjection(
    Money LineAmount,
    Guid ExpenseAccountId,
    string Description,
    Guid? CostCenterId
);