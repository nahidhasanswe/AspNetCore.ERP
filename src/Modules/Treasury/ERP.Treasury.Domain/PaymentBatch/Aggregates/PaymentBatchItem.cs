using ERP.Core.ValueObjects;
using ERP.Treasury.Domain.Shared.ValueObjects;

namespace ERP.Treasury.Domain.PaymentBatch.Aggregates;

public class PaymentBatchLine : ValueObject
{
    public Guid BatchId { get; private set; }
    public Guid InvoiceId { get; private set; }
    public Money PaymentAmount { get; private set; }

    public PaymentBatchLine(Guid batchId, Guid invoiceId, Money paymentAmount)
    {
        BatchId = batchId;
        InvoiceId = invoiceId;
        PaymentAmount = paymentAmount;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return BatchId;
        yield return InvoiceId;
        yield return PaymentAmount;
    }
}