using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Shared.Events.Events;
using ERP.Treasury.Domain.PaymentBatch.Enums;
using ERP.Treasury.Domain.Shared.ValueObjects;
using InvoiceStatus = ERP.Shared.Events.Enums.InvoiceStatus;

namespace ERP.Treasury.Domain.PaymentBatch.Aggregates;


public class PaymentBatch : AggregateRoot
{
    public DateTime CreationDate { get; private set; }
    public PaymentBatchStatus Status { get; private set; }
    public Guid PaymentAccountId { get; private set; }
    public Money TotalAmount => new(_lines.Sum(l => l.PaymentAmount.Amount), _lines.FirstOrDefault()?.PaymentAmount.Currency ?? "USD");

    private readonly List<PaymentBatchLine> _lines = new();
    public IReadOnlyCollection<PaymentBatchLine> Lines => _lines.AsReadOnly();

    private PaymentBatch() { }

    public PaymentBatch(Guid paymentAccountId) : base(Guid.NewGuid())
    {
        PaymentAccountId = paymentAccountId;
        CreationDate = DateTime.UtcNow;
        Status = PaymentBatchStatus.Open;
    }

    public void AddInvoice(Guid invoiceId, Money outstandingBalance, InvoiceStatus invoiceStatus)
    {
        if (Status != PaymentBatchStatus.Open)
            throw new DomainException("Can only add invoices to an open batch.");

        if (invoiceStatus != InvoiceStatus.Approved && invoiceStatus != InvoiceStatus.ScheduledForPayment)
            throw new DomainException("Invoice must be approved or scheduled for payment.");

        if (_lines.Any(l => l.InvoiceId == invoiceId)) return;

        _lines.Add(new PaymentBatchLine(Id, invoiceId, outstandingBalance));
    }

    public void Execute(string transactionReference)
    {
        if (Status != PaymentBatchStatus.Open)
            throw new DomainException("Can only execute an open payment batch.");
        if (!_lines.Any())
            throw new DomainException("Cannot execute an empty payment batch.");

        Status = PaymentBatchStatus.Processing;

        foreach (var line in _lines)
        {
            AddDomainEvent(new BatchInvoicePaymentEvent(
                line.InvoiceId,
                line.PaymentAmount.Amount, // Use the line's payment amount
                line.PaymentAmount.Currency,
                transactionReference,
                DateTime.UtcNow,
                PaymentAccountId
            ));
        }

        Status = PaymentBatchStatus.Completed;
    }
}