using ERP.Core;
using ERP.Core.Aggregates;
using ERP.Finance.Domain.Encumbrance.Enums;
using ERP.Finance.Domain.Encumbrance.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.Encumbrance.Aggregates;

public class Encumbrance : AuditableAggregateRoot
{
    public Guid SourceTransactionId { get; private set; } // e.g., Purchase Requisition ID
    public Money Amount { get; private set; }            // The amount reserved/committed
    public Guid GlAccountId { get; private set; }        // The budget line item account
    
    public Guid? CostCenterId { get; private set; }
    public EncumbranceType Type { get; private set; }    // Reserved (Pre-Commitment) or Committed (PO Issued)
    public EncumbranceStatus Status { get; private set; }

    private Encumbrance() { }

    public Encumbrance(Guid sourceTransactionId, Money amount, Guid glAccountId, Guid? costCenterId) : base(Guid.NewGuid())
    {
        SourceTransactionId = sourceTransactionId;
        Amount = amount;
        GlAccountId = glAccountId;
        Type = EncumbranceType.Reserved; // Starts as a reservation
        Status = EncumbranceStatus.Open;
        CostCenterId = costCenterId;
        
        // Event for GL posting to a commitment ledger
        AddDomainEvent(new EncumbranceCreatedEvent(this.Id, sourceTransactionId, amount, glAccountId, this.CostCenterId, this.Type));
    }

    /// <summary>
    /// Converts a reserved encumbrance (requisition) into a committed encumbrance (Purchase Order).
    /// </summary>
    public Result ConvertToCommitment(Money newAmount)
    {
        if (this.Status != EncumbranceStatus.Open)
            return Result.Failure("Cannot commit a closed encumbrance.");

        if (this.Type == EncumbranceType.Committed)
            return Result.Failure("Already a firm commitment.");

        this.Amount = newAmount; // Amount may change slightly when converting
        this.Type = EncumbranceType.Committed;
        
        // Event for GL adjustment if the amount changed, and for notifying the Budget Read Model
        AddDomainEvent(new EncumbranceConvertedToCommitmentEvent(this.Id, newAmount, GlAccountId, this.CostCenterId));
        
        return Result.Success();
    }

    public void Release()
    {
        if (Status != EncumbranceStatus.Open)
            throw new InvalidOperationException("Only open encumbrances can be released.");
        Status = EncumbranceStatus.Released;
        AddDomainEvent(new EncumbranceReleasedEvent(this.Id, this.Amount, this.GlAccountId, this.CostCenterId));
    }

    public void Liquidate(Guid actualTransactionId)
    {
        if (Status != EncumbranceStatus.Open && Status != EncumbranceStatus.Committed) // Assuming Committed is a status
            throw new InvalidOperationException("Only open or committed encumbrances can be liquidated.");
        Status = EncumbranceStatus.Liquidated;
        AddDomainEvent(new EncumbranceLiquidatedEvent(this.Id, this.Amount, this.GlAccountId, this.CostCenterId, actualTransactionId));
    }

    public void Close()
    {
        if (Status != EncumbranceStatus.Open && Status != EncumbranceStatus.Committed)
            throw new InvalidOperationException("Only open or committed encumbrances can be closed.");
        Status = EncumbranceStatus.Closed;
        AddDomainEvent(new EncumbranceClosedEvent(this.Id, this.Amount, this.GlAccountId, this.CostCenterId));
    }

    public void Cancel()
    {
        if (Status != EncumbranceStatus.Open && Status != EncumbranceStatus.Committed)
            throw new InvalidOperationException("Only open or committed encumbrances can be cancelled.");
        Status = EncumbranceStatus.Canceled;
        AddDomainEvent(new EncumbranceCanceledEvent(this.Id, this.Amount, this.GlAccountId, this.CostCenterId));
    }

    public void Adjust(Money newAmount)
    {
        if (Status != EncumbranceStatus.Open && Status != EncumbranceStatus.Committed)
            throw new InvalidOperationException("Only open or committed encumbrances can be adjusted.");
        
        var adjustmentAmount = new Money(newAmount.Amount - this.Amount.Amount, newAmount.Currency);
        this.Amount = newAmount;
        
        AddDomainEvent(new EncumbranceAdjustedEvent(this.Id, adjustmentAmount, this.GlAccountId, this.CostCenterId));
    }
}