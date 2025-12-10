using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public enum CustomerCreditMemoStatus
{
    Issued,
    PartiallyApplied,
    FullyApplied,
    Voided
}

public class CustomerCreditMemo : AggregateRoot
{
    public Guid BusinessUnitId { get; private set; } // New property
    public Guid CustomerId { get; private set; }
    public Money OriginalAmount { get; private set; }
    public Money AvailableAmount { get; private set; }
    public CustomerCreditMemoStatus Status { get; private set; }
    public DateTime MemoDate { get; private set; }
    public string Reason { get; private set; }

    private CustomerCreditMemo() { }

    public CustomerCreditMemo(Guid businessUnitId, Guid customerId, Money amount, DateTime memoDate, string reason) : base(Guid.NewGuid())
    {
        if (amount.Amount <= 0) throw new ArgumentException("Credit memo amount must be positive.");
        
        BusinessUnitId = businessUnitId; // Set new property
        CustomerId = customerId;
        OriginalAmount = amount;
        AvailableAmount = amount;
        MemoDate = memoDate;
        Reason = reason;
        Status = CustomerCreditMemoStatus.Issued;
        // Optionally raise an event here if creation has GL impact
    }

    public void Apply(Money amountToApply)
    {
        if (Status == CustomerCreditMemoStatus.FullyApplied || Status == CustomerCreditMemoStatus.Voided)
            throw new DomainException("Credit memo is already fully applied or voided.");

        if (amountToApply.Amount > AvailableAmount.Amount)
            throw new DomainException("Cannot apply more than the available credit amount.");

        AvailableAmount = new Money(AvailableAmount.Amount - amountToApply.Amount, AvailableAmount.Currency);

        if (AvailableAmount.Amount == 0)
            Status = CustomerCreditMemoStatus.FullyApplied;
        else
            Status = CustomerCreditMemoStatus.PartiallyApplied;
    }

    public void Void(string reason)
    {
        if (Status == CustomerCreditMemoStatus.FullyApplied || Status == CustomerCreditMemoStatus.PartiallyApplied)
            throw new DomainException("Cannot void a credit memo that has already been applied.");
        
        Status = CustomerCreditMemoStatus.Voided;
        // Add a domain event here if auditing is required for voided memos
    }
}