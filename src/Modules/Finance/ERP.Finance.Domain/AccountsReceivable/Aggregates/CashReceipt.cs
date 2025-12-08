using ERP.Core;
using ERP.Core.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Enums;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public class CashReceipt : AggregateRoot
{
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
        Guid customerId, 
        DateTime receiptDate, 
        Money receivedAmount, 
        string reference,
        Guid cashAccountId) : base(Guid.NewGuid())
    {
        CustomerId = customerId;
        ReceiptDate = receiptDate;
        TotalReceivedAmount = receivedAmount;
        TotalAppliedAmount = new Money(0m, receivedAmount.Currency);
        TransactionReference = reference;
        CashAccountId = cashAccountId;
        Status = ReceiptStatus.Unapplied;

        // Event for initial GL posting and application workflow initiation
        AddDomainEvent(new UnappliedCashCreatedEvent(
            this.Id, customerId, receivedAmount, cashAccountId, reference
        ));
    }

    public static CashReceipt Create(
        Guid customerId, 
        DateTime receiptDate, 
        Money receivedAmount, 
        string reference,
        Guid cashAccountId)
    {
        var create = new CashReceipt(customerId, receiptDate, receivedAmount, reference, cashAccountId);
        return create;
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
}