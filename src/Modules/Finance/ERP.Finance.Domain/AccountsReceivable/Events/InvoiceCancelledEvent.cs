using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using ERP.Finance.Domain.Events;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record InvoiceCancelledEvent(
    Guid InvoiceId,
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
