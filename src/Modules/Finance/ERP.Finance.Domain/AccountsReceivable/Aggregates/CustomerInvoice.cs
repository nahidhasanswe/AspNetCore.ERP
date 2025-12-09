using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.Events; // Corrected using statement for InvoiceIssuedEvent
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

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
    public decimal TotalCreditsApplied { get; private set; } // New property for credits

    public Money OutstandingBalance 
    {
        get 
        {
            var remaining = TotalAmount.Amount - TotalPaymentsReceived - TotalAmountWrittenOff - TotalCreditsApplied;
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
            TotalAmountWrittenOff = 0m,
            TotalCreditsApplied = 0m // Initialize new property
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
            this.LineItems.Select(li => new CustomerInvoiceLineItemProjection(li.LineAmount, li.RevenueAccountId, li.Description, li.CostCenterId)).ToList(), // Corrected li.CostCenterId
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

    public void Cancel(string reason)
    {
        if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.WrittenOff || Status == InvoiceStatus.Closed)
            throw new DomainException("Cannot cancel a paid, written-off, or closed invoice.");
        if (TotalPaymentsReceived > 0)
            throw new DomainException("Cannot cancel an invoice with recorded payments.");

        Status = InvoiceStatus.Cancel;

        // Raise event for GL reversal
        AddDomainEvent(new InvoiceCancelledEvent(
            this.Id,
            this.TotalAmount,
            this.ARControlAccountId,
            this.LineItems.Select(li => new CustomerInvoiceLineItemProjection(li.LineAmount, li.RevenueAccountId, li.Description, li.CostCenterId)).ToList(),
            reason,
            DateTime.UtcNow,
            this.CostCenterId
        ));
    }

    public void Adjust(Money adjustmentAmount, string reason, Guid adjustmentAccountId, Guid? costCenterId)
    {
        if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.WrittenOff || Status == InvoiceStatus.Closed || Status == InvoiceStatus.Cancel)
            throw new DomainException("Cannot adjust a paid, written-off, closed, or cancelled invoice.");
        if (adjustmentAmount.Amount == 0)
            throw new DomainException("Adjustment amount cannot be zero.");
        if (adjustmentAmount.Currency != TotalAmount.Currency)
            throw new DomainException("Adjustment currency must match invoice currency.");

        // Update TotalAmount and recalculate outstanding balance
        TotalAmount = new Money(TotalAmount.Amount + adjustmentAmount.Amount, TotalAmount.Currency);
        
        // If the adjustment makes the outstanding balance zero or negative, mark as paid/closed
        if (OutstandingBalance.Amount <= 0)
        {
            Status = InvoiceStatus.Paid; // Or InvoiceStatus.Closed if it's a final adjustment
        }

        // Raise event for GL posting
        AddDomainEvent(new InvoiceAdjustedEvent(
            this.Id,
            adjustmentAmount,
            reason,
            this.ARControlAccountId,
            adjustmentAccountId, // The GL account for the adjustment (e.g., Sales Returns, Discount Expense)
            DateTime.UtcNow,
            costCenterId
        ));
    }

    public void ApplyCreditMemo(CustomerCreditMemo creditMemo, Money amountToApply)
    {
        if (creditMemo.CustomerId != CustomerId)
            throw new DomainException("Credit memo must belong to the same customer as the invoice.");
        if (amountToApply.Amount > OutstandingBalance.Amount)
            throw new DomainException("Credit applied cannot exceed the outstanding balance of the invoice.");
        if (amountToApply.Amount <= 0)
            throw new DomainException("Amount to apply must be positive.");
        if (amountToApply.Currency != TotalAmount.Currency)
            throw new DomainException("Credit memo currency must match invoice currency.");

        TotalCreditsApplied += amountToApply.Amount;

        // If the invoice is now fully paid/credited, update status
        if (OutstandingBalance.Amount == 0)
        {
            Status = InvoiceStatus.Paid; // Or InvoiceStatus.Closed
        }

        // Raise event for GL posting (Debit AR Control, Credit Customer Credits Clearing)
        AddDomainEvent(new CustomerCreditAppliedToInvoiceEvent(
            InvoiceId: this.Id,
            CreditMemoId: creditMemo.Id,
            AmountApplied: amountToApply,
            AppliedDate: DateTime.UtcNow,
            ARControlAccountId: this.ARControlAccountId
        ));
    }

    public void UnapplyPayment(Money amountToUnapply)
    {
        if (amountToUnapply.Amount <= 0)
            throw new DomainException("Amount to unapply must be positive.");
        if (amountToUnapply.Amount > TotalPaymentsReceived)
            throw new DomainException("Cannot unapply more than has been received.");

        TotalPaymentsReceived -= amountToUnapply.Amount;

        // If the invoice was fully paid and now has an outstanding balance, revert status
        if (Status == InvoiceStatus.Paid && OutstandingBalance.Amount > 0)
        {
            Status = InvoiceStatus.Issued; // Or Overdue, depending on DueDate
        }

        // Raise event for GL reversal (Debit Cash, Credit AR)
        AddDomainEvent(new PaymentUnappliedEvent(
            InvoiceId: this.Id,
            AmountUnapplied: amountToUnapply,
            UnappliedDate: DateTime.UtcNow,
            ARControlAccountId: this.ARControlAccountId
        ));
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