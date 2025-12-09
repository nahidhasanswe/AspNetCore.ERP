using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Events;

public record VendorPaymentRecordedEvent(
    Guid InvoiceId,
    Guid VendorId,
    Guid BusinessUnitId, // New property
    Money AmountPaid,
    string TransactionReference,
    DateTime PaymentDate,
    Guid PaymentAccountId,
    Guid APControlAccountId,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}