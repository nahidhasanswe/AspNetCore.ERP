using ERP.Core.Events;
using ERP.Finance.Domain.AccountsPayable.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.Events;

public record InvoiceIssuedEvent(
    Guid InvoiceId,
    Guid BusinessUnitId,
    Money Amount,
    Guid ARControlAccountId,
    IEnumerable<CustomerInvoiceLineItemProjection> LineItems,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CustomerInvoiceLineItemProjection(
    Money LineAmount,
    Guid RevenueAccountId,
    string Description,
    Guid? CostCenterId
);