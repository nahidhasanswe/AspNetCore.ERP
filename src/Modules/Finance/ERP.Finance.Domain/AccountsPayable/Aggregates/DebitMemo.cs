using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.AccountsPayable.Events; // Added for the event
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public enum DebitMemoStatus
{
    Issued,
    Applied,
    Cancelled
}

public class DebitMemo : AggregateRoot
{
    public Guid BusinessUnitId { get; set; }
    public Guid VendorId { get; private set; }
    public Money Amount { get; private set; }
    public DebitMemoStatus Status { get; private set; }
    public DateTime MemoDate { get; private set; }
    public string Reason { get; private set; }
    public Guid APControlAccountId { get; private set; } // Added this property

    private DebitMemo() { }

    public DebitMemo(Guid vendorId, Guid businessUnitId, Money amount, DateTime memoDate, string reason, Guid apControlAccountId) : base(Guid.NewGuid())
    {
        if (amount.Amount <= 0) throw new ArgumentException("Debit memo amount must be positive.");
        
        VendorId = vendorId;
        Amount = amount;
        MemoDate = memoDate;
        Reason = reason;
        Status = DebitMemoStatus.Issued;
        APControlAccountId = apControlAccountId; // Set the AP Control Account
    }

    public void MarkAsApplied()
    {
        if (Status != DebitMemoStatus.Issued)
            throw new DomainException("Only issued debit memos can be marked as applied.");
        Status = DebitMemoStatus.Applied;

        // Raise the event for GL posting
        AddDomainEvent(new DebitMemoAppliedEvent(
            Id,
            VendorId,
            Amount,
            DateTime.UtcNow,
            APControlAccountId
        ));
    }

    public void Cancel(string reason)
    {
        if (Status != DebitMemoStatus.Issued)
            throw new DomainException("Only issued debit memos can be cancelled.");
        Status = DebitMemoStatus.Cancelled;
        // Add a domain event here if auditing is required for cancelled memos
        // If a Debit Memo has GL impact upon creation, cancelling it would need a reversing entry.
    }
}