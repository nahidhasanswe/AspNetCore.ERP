using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.AccountsPayable.Aggregates;
using ERP.Finance.Domain.AccountsPayable.Events;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public class CustomerInvoice : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public Money TotalAmount { get; private set; } // Uses shared Money Value Object
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

    public CustomerInvoice(Guid customerId, string invoiceNumber, Guid revenueAccountId, IEnumerable<CustomerInvoiceLineItem> lineItems) : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber)) throw new ArgumentException("Invoice number is required.");
        if (revenueAccountId == Guid.Empty) throw new ArgumentException("AR Control GL account is required.");
        if (lineItems == null || !lineItems.Any()) throw new DomainException("Invoice must have line items.");
        
        CustomerId = customerId;
        InvoiceNumber = invoiceNumber;
        ARControlAccountId = revenueAccountId;
        IssueDate = DateTime.UtcNow.Date;
        DueDate = IssueDate.AddDays(30); // Default Net 30
        Status = InvoiceStatus.Issued;
        TotalPaymentsReceived = 0m;
        
        _lineItems.AddRange(lineItems);
        RecalculateTotal(); // Calculate TotalAmount from lines
        
        // When an invoice is issued, the GL needs to be updated immediately (Accrual Accounting)
        // Debit: Accounts Receivable (Asset increase)
        // Credit: Revenue Account (Equity increase)
        AddDomainEvent(new InvoiceIssuedEvent(
            this.Id, 
            this.TotalAmount, 
            this.ARControlAccountId,
            this.LineItems.Select(li => new CustomerInvoiceLineItemProjection(li.LineAmount, li.RevenueAccountId, li.Description, this.CostCenterId)).ToList(),
            this.CostCenterId
        ));
    }
    
    private void RecalculateTotal()
    {
        // Logic to sum lines and check currency consistency
        var currency = _lineItems.First().LineAmount.Currency;
        TotalAmount = new Money(_lineItems.Sum(li => li.LineAmount.Amount), currency);
    }
    
    /// <summary>
    /// Core Domain Operation: Records the payment and raises a Domain Event for the GL.
    /// </summary>
    public void RecordPayment(string transactionReference, Money paymentAmount, Guid cashAccountId, DateTime paymentDate)
    {
        if (Status == InvoiceStatus.Paid)
            throw new DomainException("Invoice is already fully paid.");
        if (paymentAmount.Amount <= 0)
            throw new DomainException("Payment amount must be positive.");
        if (paymentAmount.Currency != TotalAmount.Currency)
            throw new DomainException("Payment currency must match invoice currency.");
        
        // Check if payment exceeds remaining balance
        if (paymentAmount.Amount > OutstandingBalance.Amount)
            throw new DomainException($"Payment amount {paymentAmount.Amount} exceeds outstanding balance {OutstandingBalance.Amount}.");

        // Update state
        TotalPaymentsReceived += paymentAmount.Amount;
        
        if (OutstandingBalance.Amount == 0)
        {
            Status = InvoiceStatus.Paid;
        }
        
        // Raise Event for GL (PaymentReceivedEvent must be updated)
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

        // 1. Update Aggregate State
        this.TotalAmountWrittenOff += writeOffAmount.Amount;
        this.Status = InvoiceStatus.WrittenOff; 
        
        // 2. Raise Event for GL and Credit Profile
        AddDomainEvent(new BadDebtWrittenOffEvent(
            InvoiceId: this.Id,
            WriteOffAmount: writeOffAmount,
            WriteOffDate: writeOffDate,
            ARControlAccountId: this.ARControlAccountId, 
            BadDebtExpenseAccountId: badDebtExpenseAccountId, // The destination GL account
            Reason: reason,
            CostCenterId: this.CostCenterId
        ));
    }
    
    public void ApplyDeduction(Money amount, string reasonCode, Guid deductionExpenseAccountId)
    {
        // 1. Validation: Ensure amount <= OutstandingBalance
        if (amount.Amount > this.OutstandingBalance.Amount) 
            throw new DomainException("Deduction amount exceeds outstanding balance.");
        
        // 2. State Update
        this.TotalAmountWrittenOff += amount.Amount; // Treat as a write-off/adjustment for tracking
    
        if (this.OutstandingBalance.Amount == amount.Amount)
            this.Status = InvoiceStatus.Closed; // Or DeductionApplied

        // 3. Raise Event
        AddDomainEvent(new DeductionAppliedToInvoiceEvent(
            this.Id, amount, reasonCode, this.ARControlAccountId, deductionExpenseAccountId
        ));
    }
}