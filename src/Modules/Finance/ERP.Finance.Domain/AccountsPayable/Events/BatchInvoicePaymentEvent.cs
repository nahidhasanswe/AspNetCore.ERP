using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Events;

public record BatchInvoicePaymentEvent(
    Guid InvoiceId,
    Money PaymentAmount,
    string TransactionReference,
    DateTime PaymentDate,
    Guid PaymentAccountId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}