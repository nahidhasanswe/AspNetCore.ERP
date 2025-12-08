using ERP.Core.ValueObjects;
using ERP.Treasury.Domain.Shared.ValueObjects;

namespace ERP.Treasury.Domain.PaymentBatch.Aggregates;

public class PaymentBatchItem : ValueObject
{
    public Guid InvoiceId { get; private set; }
    public Money Amount { get; private set; }
    
    public PaymentBatchItem(Guid invoiceId, Money amount)
    {
        InvoiceId = invoiceId;
        Amount = amount;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return InvoiceId;
    }
}