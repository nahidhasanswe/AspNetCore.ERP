using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.AccountsPayable.Events; // Added this
using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public enum PaymentBatchStatus
{
    Open,
    Processing,
    Completed,
    Failed
}

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

    public void AddInvoice(VendorInvoice invoice)
    {
        if (Status != PaymentBatchStatus.Open)
            throw new DomainException("Can only add invoices to an open batch.");

        if (invoice.Status != Shared.Enums.InvoiceStatus.Approved && invoice.Status != Shared.Enums.InvoiceStatus.ScheduledForPayment)
            throw new DomainException("Invoice must be approved or scheduled for payment.");

        if (_lines.Any(l => l.InvoiceId == invoice.Id)) return;

        _lines.Add(new PaymentBatchLine(Id, invoice.Id, invoice.OutstandingBalance));
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
                line.PaymentAmount, // Use the line's payment amount
                transactionReference,
                DateTime.UtcNow,
                PaymentAccountId
            ));
        }

        Status = PaymentBatchStatus.Completed;
    }
}

public class PaymentBatchLine
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
}