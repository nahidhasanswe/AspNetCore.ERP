using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.AccountsPayable.Events;
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public class VendorInvoice : AggregateRoot
{
    public Guid VendorId { get; private set; }
    public DateTime InvoiceDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public Money TotalAmount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public string InvoiceNumber { get; private set; }
    
    public Guid? CostCenterId { get; private set; }
    
    public bool IsOnHold { get; private set; } = false;
    public decimal TotalPaymentsRecorded { get; private set; } 
    
    public Guid APControlAccountId { get; private set; }
    
    public Money OutstandingBalance 
    {
        get 
        {
            var remaining = TotalAmount.Amount - TotalPaymentsRecorded;
            return new Money(Math.Max(0, remaining), TotalAmount.Currency);
        }
    }

    private readonly List<InvoiceLineItem> _lineItems = new();
    public IReadOnlyCollection<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();

    private VendorInvoice() { }

    private VendorInvoice(Guid vendorId, string invoiceNumber, DateTime invoiceDate, DateTime dueDate, Guid apControlAccountId, Guid? costCenterId, IEnumerable<InvoiceLineItem> lineItems) : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber)) throw new ArgumentException("Invoice number is required.");
        var items = lineItems.ToList();
        if (!items.Any()) throw new DomainException("A vendor invoice must have at least one line item.");

        VendorId = vendorId;
        InvoiceNumber = invoiceNumber;
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        Status = InvoiceStatus.Submitted;
        APControlAccountId = apControlAccountId;
        CostCenterId = costCenterId;

        TotalPaymentsRecorded = 0m;

        _lineItems.AddRange(items);
        RecalculateTotal();
    }

    public static VendorInvoice Create(Guid vendorId, string invoiceNumber, DateTime invoiceDate, DateTime dueDate, Guid apControlAccountId, Guid? costCenterId, IEnumerable<InvoiceLineItem> lineItems)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber)) 
            throw new ArgumentException("Invoice number is required.");
        
        var items = lineItems.ToList();
        if (!items.Any()) 
            throw new DomainException("A vendor invoice must have at least one line item.");
 
        var invoice = new VendorInvoice(vendorId, invoiceNumber, invoiceDate, dueDate, apControlAccountId, costCenterId, items);
         
        // You can raise an initial creation event here if needed, similar to the one in your Approve() method.
        // For example: invoice.AddDomainEvent(new VendorInvoiceCreatedEvent(...));
         
        return invoice;
    }
    
    public void Update(DateTime newDueDate, IEnumerable<InvoiceLineItem> newLineItems)
    {
        if (Status != InvoiceStatus.Submitted)
        {
            throw new DomainException("Only invoices in 'Submitted' status can be modified.");
        }
        
        DueDate = newDueDate;
        
        // Replace existing lines and recalculate
        _lineItems.Clear();
        _lineItems.AddRange(newLineItems);
        RecalculateTotal();
        
        // Domain Event could be raised here for auditing/integration: InvoiceDetailsUpdatedEvent
    }

    public void AddLineItem(string description, Money lineAmount, Guid expenseAccountId, Guid? costCenterId)
    {
        if (Status != InvoiceStatus.Submitted)
        {
            throw new DomainException("Cannot add line items to an invoice that is not in 'Submitted' status.");
        }
        var lineItem = new InvoiceLineItem(description, lineAmount, expenseAccountId, costCenterId);
        _lineItems.Add(lineItem);
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        if (!_lineItems.Any())
        {
            TotalAmount = new Money(0, "USD"); // Default currency, consider making this configurable
            return;
        }

        var currency = _lineItems.First().LineAmount.Currency;
        if (_lineItems.Any(li => li.LineAmount.Currency != currency))
        {
            throw new DomainException("All line items must have the same currency.");
        }

        TotalAmount = new Money(_lineItems.Sum(li => li.LineAmount.Amount), currency);
    }

    public void Approve()
    {
        if (Status != InvoiceStatus.Submitted)
            throw new DomainException("Only submitted invoices can be approved.");
        if (TotalAmount.Amount <= 0)
            throw new DomainException("Cannot approve an invoice with a zero or negative total amount.");
        
        Status = InvoiceStatus.Approved;
        
        AddDomainEvent(new VendorInvoiceApprovedEvent(
            Id,
            VendorId,
            TotalAmount,
            DateTime.UtcNow,
            LineItems: LineItems.Select(li => new 
                InvoiceLineItemProjection(
                li.LineAmount,
                li.ExpenseAccountId,
                li.Description,
                this.CostCenterId
            )).ToList()
        ));
    }

    public void SchedulePayment()
    {
        if (Status != InvoiceStatus.Approved)
            throw new DomainException("Only approved invoices can be scheduled.");
        Status = InvoiceStatus.ScheduledForPayment;
    }
    
    public void RecordPayment(Money paymentAmount, string transactionReference, DateTime paymentDate, Guid paymentAccountId, Guid apControlAccountId)
    {
        if (Status == InvoiceStatus.Paid)
            throw new DomainException("Cannot record payment on a fully paid invoice.");
        if (paymentAmount.Amount <= 0)
            throw new DomainException("Payment amount must be positive.");
        if (paymentAmount.Currency != TotalAmount.Currency)
            throw new DomainException("Payment currency must match invoice currency.");
    
        // Check if payment exceeds remaining balance
        if (paymentAmount.Amount > OutstandingBalance.Amount)
            throw new DomainException($"Payment amount {paymentAmount.Amount} exceeds outstanding balance {OutstandingBalance.Amount}.");

        // Update internal state
        TotalPaymentsRecorded += paymentAmount.Amount;

        // Raise a specific event to trigger the GL posting (Debit AP Control, Credit Cash/Bank)
        AddDomainEvent(new VendorPaymentRecordedEvent(
            Id,
            VendorId,
            paymentAmount,
            transactionReference,
            paymentDate,
            paymentAccountId, // GL account the cash came from
            apControlAccountId,
            this.CostCenterId
        ));

        // Update status if fully paid
        if (OutstandingBalance.Amount == 0)
        {
            Status = InvoiceStatus.Paid;
        }
        // If partially paid, status should remain ScheduledForPayment or Approved
    }
    
    public void Cancel(string reason)
    {
        if (TotalPaymentsRecorded > 0)
        {
            throw new DomainException("Cannot cancel an invoice with recorded payments.");
        }
        if (Status == InvoiceStatus.Cancel) return; // Idempotency
        
        var lineProjections = LineItems.Select(li => new 
            InvoiceLineItemProjection(
                li.LineAmount,
                li.ExpenseAccountId,
                li.Description,
                this.CostCenterId
            )).ToList();
        
        Status = InvoiceStatus.Cancel;
        
        // Raise event to reverse any posted GL entry (Requires a reversing GL handler)
        AddDomainEvent(new VendorInvoiceCancelledEvent(
                this.Id, 
                this.VendorId, 
                reason,
                DateTime.UtcNow,
                this.TotalAmount,
                this.APControlAccountId,
                lineProjections
            ));
    }
    
    public void PlaceOnHold()
    {
        if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.Cancel)
        {
            throw new DomainException("Cannot place a paid or cancelled invoice on hold.");
        }
        IsOnHold = true;
        AddDomainEvent(new VendorInvoiceHoldStatusChangedEvent(this.Id, true));
    }
    
    public void RemoveFromHold()
    {
        IsOnHold = false;
        AddDomainEvent(new VendorInvoiceHoldStatusChangedEvent(this.Id, false));
    }
}