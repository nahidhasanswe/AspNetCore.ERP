using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.Events;
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public class CustomerInvoice : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public Money TotalAmount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public string InvoiceNumber { get; private set; }
    public Guid ARControlAccountId { get; private set; } 
    public Guid? CostCenterId { get; private set; }
    public decimal TotalPaymentsReceived { get; private set; } 
    public decimal TotalAmountWrittenOff { get; private set; }
    
    public Money OutstandingBalance 
    {
        get 
        {
            var remaining = TotalAmount.Amount - TotalPaymentsReceived - TotalAmountWrittenOff;
            return new Money(Math.Max(0, remaining), TotalAmount.Currency);
        }
    }
    
    private readonly List<CustomerInvoiceLineItem> _lineItems = new();
    public IReadOnlyCollection<CustomerInvoiceLineItem> LineItems => _lineItems.AsReadOnly();

    private CustomerInvoice() { }

    // Static factory method to create a draft invoice
    public static CustomerInvoice CreateDraft(Guid customerId, string invoiceNumber, Guid arControlAccountId, DateTime dueDate, Guid? costCenterId, IEnumerable<CustomerInvoiceLineItem> lineItems)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber)) throw new ArgumentException("Invoice number is required.");
        if (arControlAccountId == Guid.Empty) throw new ArgumentException("AR Control GL account is required.");
        if (lineItems == null || !lineItems.Any()) throw new DomainException("Invoice must have line items.");

        var invoice = new CustomerInvoice
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            InvoiceNumber = invoiceNumber,
            ARControlAccountId = arControlAccountId,
            IssueDate = DateTime.MinValue, // Will be set upon Issue()
            DueDate = dueDate,
            Status = InvoiceStatus.Draft,
            CostCenterId = costCenterId,
            TotalPaymentsReceived = 0m,
            TotalAmountWrittenOff = 0m
        };
        invoice._lineItems.AddRange(lineItems);
        invoice.RecalculateTotal();
        return invoice;
    }

    // Method to issue a draft invoice
    public void Issue(DateTime issueDate)
    {
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("Only draft invoices can be issued.");
        if (!_lineItems.Any())
            throw new DomainException("Cannot issue an invoice without line items.");
        if (TotalAmount.Amount <= 0)
            throw new DomainException("Cannot issue an invoice with zero or negative total amount.");

        IssueDate = issueDate;
        Status = InvoiceStatus.Issued;

        // Raise Event for GL (Debit AR, Credit Revenue)
        AddDomainEvent(new InvoiceIssuedEvent(
            this.Id, 
            this.TotalAmount, 
            this.ARControlAccountId,
            this.LineItems.Select(li => new CustomerInvoiceLineItemProjection(li.LineAmount, li.RevenueAccountId, li.Description, this.CostCenterId)).ToList(),
            this.CostCenterId
        ));
    }

    // Method to update invoice header details
    public void Update(DateTime newDueDate, Guid? newCostCenterId)
    {
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("Only draft invoices can be updated.");
        
        DueDate = newDueDate;
        CostCenterId = newCostCenterId;
    }

    // Method to add a line item to a draft invoice
    public void AddLineItem(string description, Money lineAmount, Guid revenueAccountId, Guid? costCenterId)
    {
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("Cannot add line items to an invoice that is not in 'Draft' status.");
        
        _lineItems.Add(new CustomerInvoiceLineItem(description, lineAmount, revenueAccountId, costCenterId));
        RecalculateTotal();
    }

    // Method to update an existing line item on a draft invoice
    public void UpdateLineItem(Guid lineItemId, string description, Money lineAmount, Guid revenueAccountId, Guid? costCenterId)
    {
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("Cannot update line items on an invoice that is not in 'Draft' status.");
        
        var itemToUpdate = _lineItems.FirstOrDefault(li => li.Id == lineItemId);
        if (itemToUpdate == null)
            throw new DomainException($"Invoice line item with ID {lineItemId} not found.");

        itemToUpdate.Update(description, lineAmount, revenueAccountId, costCenterId);
        RecalculateTotal();
    }

    // Method to remove a line item from a draft invoice
    public void RemoveLineItem(Guid lineItemId)
    {
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("Cannot remove line items from an invoice that is not in 'Draft' status.");
        
        var itemToRemove = _lineItems.FirstOrDefault(li => li.Id == lineItemId);
        if (itemToRemove == null)
            throw new DomainException($"Invoice line item with ID {lineItemId} not found.");

        _lineItems.Remove(itemToRemove);
        RecalculateTotal();
    }
    
    private void RecalculateTotal()
    {
        if (!_lineItems.Any())
        {
            TotalAmount = new Money(0m, "USD"); // Default currency
            return;
        }
        var currency = _lineItems.First().LineAmount.Currency;
        if (_lineItems.Any(li => li.LineAmount.Currency != currency))
        {
            throw new DomainException("All invoice line items must have the same currency.");
        }
        TotalAmount = new Money(_lineItems.Sum(li => li.LineAmount.Amount), currency);
    }
    
    public void RecordPayment(string transactionReference, Money paymentAmount, Guid cashAccountId, DateTime paymentDate)
    {
        if (Status == InvoiceStatus.Paid)
            throw new DomainException("Invoice is already fully paid.");
        if (paymentAmount.Amount <= 0)
            throw new DomainException("Payment amount must be positive.");
        if (paymentAmount.Currency != TotalAmount.Currency)
            throw new DomainException("Payment currency must match invoice currency.");
        
        if (paymentAmount.Amount > OutstandingBalance.Amount)
            throw new DomainException($"Payment amount {paymentAmount.Amount} exceeds outstanding balance {OutstandingBalance.Amount}.");

        TotalPaymentsReceived += paymentAmount.Amount;
        
        if (OutstandingBalance.Amount == 0)
        {
            Status = InvoiceStatus.Paid;
        }
        
        AddDomainEvent(new PaymentReceivedEvent(
            this.Id, 
            paymentAmount, 
            paymentDate,
            transactionReference,
            this.ARControlAccountId,
            cashAccountId,
            this.CostCenterId)
        );
    }
    
    public void WriteOff(DateTime writeOffDate, string reason, Guid badDebtExpenseAccountId)
    {
        if (this.Status == InvoiceStatus.WrittenOff || this.Status == InvoiceStatus.Paid)
        {
            throw new DomainException($"Invoice {this.InvoiceNumber} is already {this.Status}. Cannot write off.");
        }

        var writeOffAmount = this.OutstandingBalance;
        
        if (writeOffAmount.Amount <= 0)
        {
            throw new DomainException("Invoice has no positive outstanding balance to write off.");
        }

        this.TotalAmountWrittenOff += writeOffAmount.Amount;
        this.Status = InvoiceStatus.WrittenOff; 
        
        AddDomainEvent(new BadDebtWrittenOffEvent(
            InvoiceId: this.Id,
            WriteOffAmount: writeOffAmount,
            WriteOffDate: writeOffDate,
            ARControlAccountId: this.ARControlAccountId, 
            BadDebtExpenseAccountId: badDebtExpenseAccountId,
            Reason: reason,
            CostCenterId: this.CostCenterId
        ));
    }
    
    public void ApplyDeduction(Money amount, string reasonCode, Guid deductionExpenseAccountId)
    {
        if (amount.Amount > this.OutstandingBalance.Amount) 
            throw new DomainException("Deduction amount exceeds outstanding balance.");
        
        this.TotalAmountWrittenOff += amount.Amount;
    
        if (this.OutstandingBalance.Amount == amount.Amount)
            this.Status = InvoiceStatus.Closed;

        AddDomainEvent(new DeductionAppliedToInvoiceEvent(
            this.Id, amount, reasonCode, this.ARControlAccountId, deductionExpenseAccountId
        ));
    }
}