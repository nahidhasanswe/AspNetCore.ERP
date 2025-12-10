using ERP.Core;
using ERP.Core.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Enums;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using ERP.Core.Exceptions;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public class CashReceipt : AggregateRoot
{
    public Guid BusinessUnitId { get; private set; } // New property
    public Guid CustomerId { get; private set; } 
    public DateTime ReceiptDate { get; private set; }
    public Money TotalReceivedAmount { get; private set; }
    public Money TotalAppliedAmount { get; private set; } // Amount allocated to invoices/deductions
    public string TransactionReference { get; private set; } // Bank statement reference
    public Guid CashAccountId { get; private set; } // The GL account that was DEBITED (Cash in Bank)
    public ReceiptStatus Status { get; private set; }

    public Money UnappliedAmount => new Money(
        TotalReceivedAmount.Amount - TotalAppliedAmount.Amount,
        TotalReceivedAmount.Currency
    );

    private CashReceipt() { }

    private CashReceipt(
        Guid businessUnitId,
        Guid customerId, 
        DateTime receiptDate, 
        Money receivedAmount, 
        string reference,
        Guid cashAccountId) : base(Guid.NewGuid())
    {
        BusinessUnitId = businessUnitId; // Set new property
        CustomerId = customerId;
        ReceiptDate = receiptDate;
        TotalReceivedAmount = receivedAmount;
        TotalAppliedAmount = new Money(0m, receivedAmount.Currency);
        TransactionReference = reference;
        CashAccountId = cashAccountId;
        Status = ReceiptStatus.Unapplied;

        // Event for initial GL posting and application workflow initiation
        AddDomainEvent(new UnappliedCashCreatedEvent(
            this.Id, customerId, this.BusinessUnitId, receivedAmount, cashAccountId, reference
        ));
    }

    public static CashReceipt Create(
        Guid businessUnitId,
        Guid customerId, 
        DateTime receiptDate, 
        Money receivedAmount, 
        string reference,
        Guid cashAccountId)
    {
        var create = new CashReceipt(businessUnitId, customerId, receiptDate, receivedAmount, reference, cashAccountId);
        return create;
    }

    public void Update(DateTime newReceiptDate, Money newReceivedAmount, string newTransactionReference, Guid newCashAccountId)
    {
        if (Status != ReceiptStatus.Unapplied)
            throw new DomainException("Only unapplied cash receipts can be updated.");
        if (newReceivedAmount.Amount < TotalAppliedAmount.Amount)
            throw new DomainException("New received amount cannot be less than already applied amount.");

        ReceiptDate = newReceiptDate;
        TotalReceivedAmount = newReceivedAmount;
        TransactionReference = newTransactionReference;
        CashAccountId = newCashAccountId;
    }

    public void Cancel()
    {
        if (Status != ReceiptStatus.Unapplied)
            throw new DomainException("Only unapplied cash receipts can be cancelled.");
        
        Status = ReceiptStatus.Cancelled;
        // Raise event for GL reversal of the initial UnappliedCashCreatedEvent
        AddDomainEvent(new CashReceiptCancelledEvent(
            this.Id,
            this.CustomerId,
            this.BusinessUnitId,
            this.TotalReceivedAmount,
            this.CashAccountId,
            this.TransactionReference,
            DateTime.UtcNow
        ));
    }

    public void Refund(Money refundAmount, Guid refundCashAccountId, string refundReference)
    {
        if (Status != ReceiptStatus.Unapplied && Status != ReceiptStatus.PartiallyApplied)
            throw new DomainException("Only unapplied or partially applied cash can be refunded.");
        if (refundAmount.Amount <= 0)
            throw new DomainException("Refund amount must be positive.");
        if (refundAmount.Amount > UnappliedAmount.Amount)
            throw new DomainException("Cannot refund more than the unapplied amount.");

        TotalAppliedAmount = new Money(TotalAppliedAmount.Amount + refundAmount.Amount, TotalAppliedAmount.Currency); // Treat refund as an application to reduce unapplied amount

        if (UnappliedAmount.Amount == 0)
        {
            Status = ReceiptStatus.FullyApplied; // All unapplied amount is now either applied to invoices or refunded
        }

        AddDomainEvent(new CashReceiptRefundedEvent(
            this.Id,
            this.CustomerId,
            this.BusinessUnitId,
            refundAmount,
            this.CashAccountId, // Original cash account
            refundCashAccountId, // Account from which refund is made
            refundReference,
            DateTime.UtcNow
        ));
    }

    public void Reverse(string reason)
    {
        if (Status == ReceiptStatus.Unapplied || Status == ReceiptStatus.Cancelled)
            throw new DomainException("Only applied or partially applied cash receipts can be reversed.");
        
        Status = ReceiptStatus.Reversed;
        // Raise event for GL reversal of all associated transactions
        AddDomainEvent(new CashReceiptReversedEvent(
            this.Id,
            this.CustomerId,
            this.BusinessUnitId,
            this.TotalReceivedAmount,
            this.CashAccountId,
            this.TransactionReference,
            reason,
            DateTime.UtcNow
        ));
    }

    /// <summary>
    /// Reduces the unapplied balance by applying cash to an invoice or deduction.
    /// </summary>
    public Result ApplyCash(Money amountToApply)
    {
        if (amountToApply.Amount <= 0)
            return Result.Failure("Amount to apply must be positive.");
            
        if (amountToApply.Amount > this.UnappliedAmount.Amount)
            return Result.Failure("Cannot apply more cash than the unapplied balance.");

        TotalAppliedAmount = new Money(
            TotalAppliedAmount.Amount + amountToApply.Amount,
            TotalAppliedAmount.Currency
        );
        
        if (this.UnappliedAmount.Amount == 0)
        {
            Status = ReceiptStatus.FullyApplied;
        } 
        else if (Status == ReceiptStatus.Unapplied)
        {
            Status = ReceiptStatus.PartiallyApplied;
        }

        // Note: No event needed here; the application event is triggered by the Invoice/Deduction commands.
        return Result.Success();
    }

    /// <summary>
    /// Increases the unapplied balance by reversing a cash application.
    /// </summary>
    public Result UnapplyCash(Money amountToUnapply)
    {
        if (amountToUnapply.Amount <= 0)
            return Result.Failure("Amount to unapply must be positive.");
        
        if (amountToUnapply.Amount > TotalAppliedAmount.Amount)
            return Result.Failure("Cannot unapply more cash than has been applied.");

        TotalAppliedAmount = new Money(
            TotalAppliedAmount.Amount - amountToUnapply.Amount,
            TotalAppliedAmount.Currency
        );

        if (TotalAppliedAmount.Amount == 0)
        {
            Status = ReceiptStatus.Unapplied;
        }
        else if (Status == ReceiptStatus.FullyApplied)
        {
            Status = ReceiptStatus.PartiallyApplied;
        }

        // Note: No event needed here; the unapplication event is triggered by the Invoice/Deduction commands.
        return Result.Success();
    }
}