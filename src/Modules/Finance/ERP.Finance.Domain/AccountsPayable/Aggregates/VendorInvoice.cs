using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.AccountsPayable.Events;
using ERP.Finance.Domain.AccountsPayable.Services;
using ERP.Finance.Domain.FixedAssetManagement.Aggregates; // For DepreciationSchedule
using ERP.Finance.Domain.Shared.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERP.Finance.Domain.AccountsPayable.Enums;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public class VendorInvoice : AggregateRoot
{
    public Guid BusinessUnitId { get; private set; } // New property
    public Guid VendorId { get; private set; }
    public DateTime InvoiceDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public Money TotalAmount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public string InvoiceNumber { get; private set; }
    public Guid APControlAccountId { get; private set; }
    public Guid? CostCenterId { get; private set; }
    public bool IsOnHold { get; private set; } = false;
    public decimal TotalPaymentsRecorded { get; private set; }
    public decimal TotalCreditsApplied { get; private set; } // New property
    public Guid? PurchaseOrderId { get; private set; }
    public InvoiceMatchingStatus MatchingStatus { get; private set; }

    private readonly List<Guid> _approverIds = new();
    public IReadOnlyCollection<Guid> ApproverIds => _approverIds.AsReadOnly();

    public Money OutstandingBalance => new(Math.Max(0, TotalAmount.Amount - TotalPaymentsRecorded - TotalCreditsApplied), TotalAmount.Currency);

    private readonly List<InvoiceLineItem> _lineItems = new();
    public IReadOnlyCollection<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();

    private VendorInvoice() { }
    
    private VendorInvoice(Guid businessUnitId, Guid vendorId, string invoiceNumber, DateTime invoiceDate, DateTime dueDate, Guid apControlAccountId, Guid? costCenterId, IEnumerable<InvoiceLineItem> lineItems) : base(Guid.NewGuid())
    {
        BusinessUnitId = businessUnitId; // Set new property
        VendorId = vendorId;
        InvoiceNumber = invoiceNumber;
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        Status = InvoiceStatus.Submitted;
        APControlAccountId = apControlAccountId;
        CostCenterId = costCenterId;
        MatchingStatus = InvoiceMatchingStatus.NotMatched;
        TotalCreditsApplied = 0;
        _lineItems.AddRange(lineItems);
        RecalculateTotal();
    }
    
    public static VendorInvoice CreateNonPOInvoice(Guid businessUnitId, Guid vendorId, string invoiceNumber, DateTime invoiceDate, DateTime dueDate, Guid apControlAccountId, Guid? costCenterId, IEnumerable<InvoiceLineItem> lineItems)
    {
        return new VendorInvoice(businessUnitId, vendorId, invoiceNumber, invoiceDate, dueDate, apControlAccountId, costCenterId, lineItems);
    }
    
    public static VendorInvoice CreateFromPO(Guid businessUnitId, Guid purchaseOrderId, string invoiceNumber, DateTime invoiceDate, DateTime dueDate, Guid apControlAccountId, IEnumerable<InvoiceLineItem> lineItems)
    {
        // For PO invoices, the vendor is derived from the PO.
        // We'd typically load the PO here to get the VendorId.
        // For simplicity, we'll assume the caller provides it implicitly for now.
        var poInvoice = new VendorInvoice
        {
            Id = Guid.NewGuid(),
            BusinessUnitId = businessUnitId, // Set new property
            PurchaseOrderId = purchaseOrderId,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = invoiceDate,
            DueDate = dueDate,
            APControlAccountId = apControlAccountId,
            Status = InvoiceStatus.Submitted,
            MatchingStatus = InvoiceMatchingStatus.NotMatched
        };
        poInvoice._lineItems.AddRange(lineItems);
        poInvoice.RecalculateTotal();
        return poInvoice;
    }

    public void MarkAsFixedAssetAcquisition(
        string assetTagNumber,
        string assetDescription,
        DateTime acquisitionDate,
        Money acquisitionCost,
        Guid assetAccountId,
        Guid depreciationExpenseAccountId,
        Guid accumulatedDepreciationAccountId,
        DepreciationMethod depreciationMethod,
        int usefulLifeYears,
        decimal salvageValue,
        Guid? costCenterId)
    {
        if (Status != InvoiceStatus.Approved && Status != InvoiceStatus.Paid)
            throw new DomainException("Only approved or paid invoices can be marked as fixed asset acquisitions.");
        
        // This method assumes the invoice line item corresponding to the asset has been identified.
        // For simplicity, we're taking the asset details directly.
        // In a real system, you might link this to a specific InvoiceLineItem.

        AddDomainEvent(new FixedAssetAcquiredViaInvoiceEvent(
            this.Id,
            this.VendorId,
            this.BusinessUnitId,
            assetTagNumber,
            assetDescription,
            acquisitionDate,
            acquisitionCost,
            assetAccountId,
            depreciationExpenseAccountId,
            accumulatedDepreciationAccountId,
            depreciationMethod,
            usefulLifeYears,
            salvageValue,
            costCenterId
        ));
    }

    public void ApplyCredit(CreditMemo creditMemo, Money amountToApply)
    {
        if (creditMemo.VendorId != VendorId)
            throw new DomainException("Credit memo must belong to the same vendor as the invoice.");
        if (amountToApply.Amount > OutstandingBalance.Amount)
            throw new DomainException("Credit applied cannot exceed the outstanding balance of the invoice.");

        creditMemo.Apply(amountToApply);
        TotalCreditsApplied += amountToApply.Amount;

        AddDomainEvent(new VendorInvoiceCreditAppliedEvent(
            Id,
            creditMemo.Id,
            this.BusinessUnitId,
            amountToApply,
            DateTime.UtcNow,
            APControlAccountId
        ));

        if (OutstandingBalance.Amount == 0)
        {
            Status = InvoiceStatus.Paid;
        }
    }
    
    // ... (rest of the existing methods: Approve, MatchToPO, RecordPayment, etc.)
    public async Task Approve(Guid approverId, IApprovalService approvalService)
    {
        if (Status != InvoiceStatus.Submitted && Status != InvoiceStatus.PendingApproval)
            throw new DomainException("Only submitted or pending approval invoices can be approved.");

        if (!_approverIds.Contains(approverId))
        {
            _approverIds.Add(approverId);
        }

        if (await approvalService.HasSufficientApproval(this, approverId))
        {
            Status = InvoiceStatus.Approved;
            AddDomainEvent(new VendorInvoiceApprovedEvent(
                Id,
                VendorId,
                this.BusinessUnitId,
                TotalAmount,
                DateTime.UtcNow,
                APControlAccountId,
                LineItems.Select(li => new InvoiceLineItemProjection(li.LineAmount, li.ExpenseAccountId, li.Description, CostCenterId)).ToList()
            ));
        }
        else
        {
            Status = InvoiceStatus.PendingApproval;
        }
    }
    public void MatchToPO(PurchaseOrder po, bool perform3WayMatch)
    {
        if (PurchaseOrderId == null || PurchaseOrderId != po.Id)
            throw new DomainException("This invoice is not linked to the specified purchase order.");

        foreach (var invoiceLine in _lineItems)
        {
            var poLine = po.Lines.FirstOrDefault(l => l.Description == invoiceLine.Description); 
            if (poLine == null) throw new DomainException($"Invoice line '{invoiceLine.Description}' not found on PO.");

            if (invoiceLine.LineAmount.Amount > poLine.TotalPrice.Amount)
                throw new DomainException("Invoice amount exceeds PO amount for line: " + invoiceLine.Description);
        }
        MatchingStatus = InvoiceMatchingStatus.Matched2Way;

        if (perform3WayMatch)
        {
            foreach (var invoiceLine in _lineItems)
            {
                var poLine = po.Lines.First(l => l.Description == invoiceLine.Description);
                var receivedQty = po.GetReceivedQuantity(poLine.Id);
                
                if (invoiceLine.LineAmount.Amount / poLine.UnitPrice.Amount > receivedQty)
                    throw new DomainException($"Invoice quantity for '{invoiceLine.Description}' exceeds received quantity.");
            }
            MatchingStatus = InvoiceMatchingStatus.Matched3Way;
        }
    }

    private void RecalculateTotal()
    {
        if (!_lineItems.Any())
        {
            TotalAmount = new Money(0, "USD");
            return;
        }
        var currency = _lineItems.First().LineAmount.Currency;
        if (_lineItems.Any(li => li.LineAmount.Currency != currency))
        {
            throw new DomainException("All line items must have the same currency.");
        }
        TotalAmount = new Money(_lineItems.Sum(li => li.LineAmount.Amount), currency);
    }
    public void Update(DateTime newDueDate, IEnumerable<InvoiceLineItem> newLineItems)
    {
        if (Status != InvoiceStatus.Submitted)
        {
            throw new DomainException("Only invoices in 'Submitted' status can be modified.");
        }
        
        DueDate = newDueDate;
        
        _lineItems.Clear();
        _lineItems.AddRange(newLineItems);
        RecalculateTotal();
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
    
        if (paymentAmount.Amount > OutstandingBalance.Amount)
            throw new DomainException($"Payment amount {paymentAmount.Amount} exceeds outstanding balance {OutstandingBalance.Amount}.");

        TotalPaymentsRecorded += paymentAmount.Amount;

        AddDomainEvent(new VendorPaymentRecordedEvent(
            Id,
            VendorId,
            this.BusinessUnitId,
            paymentAmount,
            transactionReference,
            paymentDate,
            paymentAccountId,
            apControlAccountId,
            this.CostCenterId
        ));

        if (OutstandingBalance.Amount == 0)
        {
            Status = InvoiceStatus.Paid;
        }
    }
    
    public void Cancel(string reason)
    {
        if (TotalPaymentsRecorded > 0)
        {
            throw new DomainException("Cannot cancel an invoice with recorded payments.");
        }
        if (Status == InvoiceStatus.Cancel) return;
        
        var lineProjections = LineItems.Select(li => new 
            InvoiceLineItemProjection(
                li.LineAmount,
                li.ExpenseAccountId,
                li.Description,
                this.CostCenterId
            )).ToList();
        
        Status = InvoiceStatus.Cancel;
        
        AddDomainEvent(new VendorInvoiceCancelledEvent(
                this.Id, 
                this.VendorId, 
                this.BusinessUnitId,
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