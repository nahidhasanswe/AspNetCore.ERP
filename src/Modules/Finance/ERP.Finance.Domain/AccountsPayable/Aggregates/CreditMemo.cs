using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public enum CreditMemoStatus
{
    Available,
    PartiallyApplied,
    FullyApplied,
    Voided
}

public class CreditMemo : AggregateRoot
{
    public Guid BusinessUnitId { get; set; }
    public Guid VendorId { get; private set; }
    public Money OriginalAmount { get; private set; }
    public Money AvailableAmount { get; private set; }
    public CreditMemoStatus Status { get; private set; }
    public DateTime MemoDate { get; private set; }
    public string Reason { get; private set; }

    private CreditMemo() { }

    public CreditMemo(Guid vendorId, Guid businessUnitId, Money amount, DateTime memoDate, string reason) : base(Guid.NewGuid())
    {
        if (amount.Amount <= 0) throw new ArgumentException("Credit memo amount must be positive.");
        
        VendorId = vendorId;
        OriginalAmount = amount;
        AvailableAmount = amount;
        MemoDate = memoDate;
        Reason = reason;
        Status = CreditMemoStatus.Available;
        BusinessUnitId = businessUnitId;
    }

    public void Apply(Money amountToApply)
    {
        if (Status == CreditMemoStatus.FullyApplied || Status == CreditMemoStatus.Voided)
            throw new DomainException("Credit memo is already fully applied or voided.");

        if (amountToApply.Amount > AvailableAmount.Amount)
            throw new DomainException("Cannot apply more than the available credit amount.");

        AvailableAmount = new Money(AvailableAmount.Amount - amountToApply.Amount, AvailableAmount.Currency);

        if (AvailableAmount.Amount == 0)
            Status = CreditMemoStatus.FullyApplied;
        else
            Status = CreditMemoStatus.PartiallyApplied;
    }

    public void Void(string reason)
    {
        if (Status == CreditMemoStatus.FullyApplied || Status == CreditMemoStatus.PartiallyApplied)
            throw new DomainException("Cannot void a credit memo that has already been applied.");
        
        Status = CreditMemoStatus.Voided;
        // Add a domain event here if auditing is required for voided memos
    }
}