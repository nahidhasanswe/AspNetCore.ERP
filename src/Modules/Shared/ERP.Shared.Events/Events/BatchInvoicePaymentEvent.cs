using ERP.Core.Events;

namespace ERP.Shared.Events.Events;

public record BatchInvoicePaymentEvent(
    Guid InvoiceId,
    decimal PaymentAmount,
    string PaymentAmountCurrency,
    string TransactionReference,
    DateTime PaymentDate,
    Guid PaymentAccountId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}