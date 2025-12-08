using ERP.Core.Events;

namespace ERP.Finance.Domain.AccountsPayable.Events;

public record VendorInvoiceHoldStatusChangedEvent(
    Guid InvoiceId,
    bool IsNowOnHold // True if placed on hold, False if removed from hold
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}