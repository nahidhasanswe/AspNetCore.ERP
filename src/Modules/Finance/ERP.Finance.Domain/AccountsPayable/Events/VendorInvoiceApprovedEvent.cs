using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Domain.AccountsPayable.Events;

public record VendorInvoiceApprovedEvent(
    Guid InvoiceId,
    Guid VendorId,
    Money TotalAmount,
    DateTime ApprovalDate,
    Guid APControlAccountId, // Added this
    IEnumerable<InvoiceLineItemProjection> LineItems
) : IDomainEvent
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