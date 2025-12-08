using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Events;

public record VendorInvoiceCreditAppliedEvent(
    Guid InvoiceId,
    Guid CreditMemoId,
    Money AmountApplied,
    DateTime AppliedDate,
    Guid APControlAccountId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}