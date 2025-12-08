using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Treasury.Domain.PaymentBatch.Enums;
using ERP.Treasury.Domain.Shared.ValueObjects;

namespace ERP.Treasury.Domain.PaymentBatch.Aggregates;

public class PaymentBatch : AggregateRoot
{
    public string BatchReference { get; private set; }
    public DateTime CreationDate { get; private set; }
    public PaymentBatchStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; }
    public string PaymentMethodCode { get; private set; }
    
    private readonly List<PaymentBatchItem> _items = new();
    public IReadOnlyCollection<PaymentBatchItem> Items => _items.AsReadOnly();

    private PaymentBatch() { }
    
    public PaymentBatch(string reference, string currency, string methodCode) : base(Guid.NewGuid())
    {
        BatchReference = reference;
        CreationDate = DateTime.UtcNow;
        Currency = currency;
        PaymentMethodCode = methodCode;
        Status = PaymentBatchStatus.Open;
        TotalAmount = 0m;
    }

    public void AddInvoice(Guid invoiceId, Money amount)
    {
        if (Status != PaymentBatchStatus.Open)
            throw new DomainException("Cannot add invoices to a closed or executed batch.");
        if (amount.Currency != this.Currency)
            throw new DomainException($"Cannot mix currencies in batch {BatchReference}. Expected {Currency}.");
        
        _items.Add(new PaymentBatchItem(invoiceId, amount));
        TotalAmount += amount.Amount;
    }
    
    public void SubmitForExecution()
    {
        if (Status != PaymentBatchStatus.Open)
            throw new DomainException("Only open batches can be submitted.");
        if (!_items.Any())
            throw new DomainException("Cannot submit an empty batch.");
            
        Status = PaymentBatchStatus.Submitted;
    }

    public void MarkExecuted(string bankFileReference)
    {
        if (Status != PaymentBatchStatus.Submitted)
            throw new DomainException("Only submitted batches can be executed.");
            
        Status = PaymentBatchStatus.Executed;
        
        // Raise event to trigger GL posting for the entire batch amount
        // AddDomainEvent(new PaymentBatchExecutedEvent(
        //     Id, 
        //     BatchReference, 
        //     new Money(TotalAmount, Currency),
        //     PaymentMethodCode,
        //     bankFileReference,
        //     Items.Select(i => i.InvoiceId).ToList()
        // ));
    }
}