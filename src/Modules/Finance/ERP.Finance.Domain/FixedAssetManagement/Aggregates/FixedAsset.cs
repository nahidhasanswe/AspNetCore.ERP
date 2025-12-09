using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.FixedAssetManagement.Enums;
using ERP.Finance.Domain.FixedAssetManagement.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.FixedAssetManagement.Aggregates;

public class FixedAsset : AggregateRoot
{
    public string TagNumber { get; private set; }
    public string Description { get; private set; }
    public DateTime AcquisitionDate { get; private set; }
    public Money AcquisitionCost { get; private set; }
    public Guid AssetAccountId { get; private set; } // GL Account (e.g., Computer Equipment)
    public Guid DepreciationExpenseAccountId { get; private set; } // GL Account (e.g., Depreciation Expense)
    public Guid AccumulatedDepreciationAccountId { get; private set; } // GL Account (e.g., Acc. Dep. - Equipment)
    public DepreciationSchedule Schedule { get; private set; }
    
    public Guid? CostCenterId { get; private set; }
    public string Location { get; private set; } // New property for physical location
    public decimal TotalAccumulatedDepreciation { get; private set; } // Tracks depreciation posted so far
    public FixedAssetStatus Status { get; private set; } // New property

    private FixedAsset() { }

    public FixedAsset(string tagNumber, string description, Money cost, DateTime date, 
                      Guid assetId, Guid expenseId, Guid accumulatedId, DepreciationSchedule schedule, Guid? costCenterId, string location) 
                      : base(Guid.NewGuid())
    {
        if (cost.Amount <= 0) throw new DomainException("Acquisition cost must be positive.");
        
        TagNumber = tagNumber;
        Description = description;
        AcquisitionCost = cost;
        AcquisitionDate = date.Date;
        AssetAccountId = assetId;
        DepreciationExpenseAccountId = expenseId;
        AccumulatedDepreciationAccountId = accumulatedId;
        Schedule = schedule;
        CostCenterId = costCenterId;
        Location = location;
        TotalAccumulatedDepreciation = 0m;
        Status = FixedAssetStatus.Active; // Initial status
    }

    public void Update(string description, Guid assetAccountId, Guid depreciationExpenseAccountId, Guid accumulatedDepreciationAccountId, Guid? costCenterId, string location)
    {
        if (Status == FixedAssetStatus.Disposed || Status == FixedAssetStatus.Retired)
            throw new DomainException("Cannot update a disposed or retired asset.");
        
        Description = description;
        AssetAccountId = assetAccountId;
        DepreciationExpenseAccountId = depreciationExpenseAccountId;
        AccumulatedDepreciationAccountId = accumulatedDepreciationAccountId;
        CostCenterId = costCenterId;
        Location = location;
        // Optionally raise an AssetUpdatedEvent
    }

    public void Revalue(DateTime revaluationDate, Money newAcquisitionCost, Guid revaluationGainLossAccountId)
    {
        if (Status == FixedAssetStatus.Disposed || Status == FixedAssetStatus.Retired)
            throw new DomainException("Cannot revalue a disposed or retired asset.");
        if (newAcquisitionCost.Amount <= 0)
            throw new DomainException("New acquisition cost must be positive.");
        if (newAcquisitionCost.Currency != AcquisitionCost.Currency)
            throw new DomainException("Revaluation currency must match asset currency.");

        Money oldAcquisitionCost = AcquisitionCost;
        AcquisitionCost = newAcquisitionCost;
        
        // Reset accumulated depreciation for simplicity, or adjust proportionally
        // In a real system, this would be more complex, involving reversal of old depreciation.
        decimal oldTotalAccumulatedDepreciation = TotalAccumulatedDepreciation;
        TotalAccumulatedDepreciation = 0m; 

        AddDomainEvent(new AssetRevaluedEvent(
            this.Id,
            revaluationDate,
            oldAcquisitionCost,
            newAcquisitionCost,
            oldTotalAccumulatedDepreciation,
            revaluationGainLossAccountId,
            this.AssetAccountId,
            this.AccumulatedDepreciationAccountId,
            this.CostCenterId
        ));
    }

    public void Transfer(Guid newCostCenterId, DateTime transferDate)
    {
        if (Status == FixedAssetStatus.Disposed || Status == FixedAssetStatus.Retired)
            throw new DomainException("Cannot transfer a disposed or retired asset.");
        if (newCostCenterId == CostCenterId)
            throw new DomainException("Asset is already in the specified cost center.");

        Guid? oldCostCenterId = CostCenterId;
        CostCenterId = newCostCenterId;

        AddDomainEvent(new AssetTransferredEvent(
            this.Id,
            transferDate,
            oldCostCenterId,
            newCostCenterId,
            this.AssetAccountId,
            this.AccumulatedDepreciationAccountId, // Pass AccumulatedDepreciationAccountId
            this.AcquisitionCost,
            this.TotalAccumulatedDepreciation
        ));
    }

    public void Impair(DateTime impairmentDate, Money impairmentLossAmount, Guid impairmentLossAccountId)
    {
        if (Status == FixedAssetStatus.Disposed || Status == FixedAssetStatus.Retired)
            throw new DomainException("Cannot impair a disposed or retired asset.");
        if (impairmentLossAmount.Amount <= 0)
            throw new DomainException("Impairment loss amount must be positive.");
        if (impairmentLossAmount.Amount > (AcquisitionCost.Amount - TotalAccumulatedDepreciation))
            throw new DomainException("Impairment loss cannot exceed current book value.");

        // Reduce the book value by the impairment loss
        AcquisitionCost = new Money(AcquisitionCost.Amount - impairmentLossAmount.Amount, AcquisitionCost.Currency);
        
        AddDomainEvent(new AssetImpairedEvent(
            this.Id,
            impairmentDate,
            impairmentLossAmount,
            impairmentLossAccountId,
            this.AssetAccountId,
            this.AccumulatedDepreciationAccountId,
            this.CostCenterId
        ));
    }

    public void Dispose(DateTime disposalDate, Money proceeds, Guid gainLossAccountId)
    {
        if (Status == FixedAssetStatus.Disposed || Status == FixedAssetStatus.Retired)
            throw new DomainException("Asset is already disposed or retired.");
        
        Status = FixedAssetStatus.Disposed;
        // Calculate gain/loss
        decimal bookValue = AcquisitionCost.Amount - TotalAccumulatedDepreciation;
        decimal gainLossAmount = proceeds.Amount - bookValue;

        AddDomainEvent(new AssetDisposedEvent(
            this.Id,
            disposalDate,
            AcquisitionCost,
            TotalAccumulatedDepreciation,
            proceeds,
            gainLossAmount,
            AssetAccountId,
            AccumulatedDepreciationAccountId,
            gainLossAccountId,
            CostCenterId
        ));
    }

    public void Retire(DateTime retirementDate, string reason)
    {
        if (Status == FixedAssetStatus.Disposed || Status == FixedAssetStatus.Retired)
            throw new DomainException("Asset is already disposed or retired.");
        
        Status = FixedAssetStatus.Retired;
        // Raise an event for GL posting (e.g., write off remaining book value)
        AddDomainEvent(new AssetRetiredEvent(
            this.Id,
            retirementDate,
            AcquisitionCost,
            TotalAccumulatedDepreciation,
            AssetAccountId,
            AccumulatedDepreciationAccountId,
            reason,
            CostCenterId
        ));
    }

    /// <summary>
    /// Core Domain Operation: Calculates and records depreciation for a period.
    /// </summary>
    public void Depreciate(DateTime periodDate, out Money depreciationAmount)
    {
        if (periodDate < AcquisitionDate) throw new DomainException("Cannot depreciate before acquisition date.");
        if (Status == FixedAssetStatus.Disposed || Status == FixedAssetStatus.Retired)
            throw new DomainException("Cannot depreciate a disposed or retired asset.");
        
        // 1. Calculate the depreciation amount for the current period
        // (This would involve complex logic to handle partial months, period vs. annual, etc.)
        decimal annualDepreciation = Schedule.CalculateAnnualDepreciation(AcquisitionCost.Amount);
        decimal monthlyDepreciation = annualDepreciation / 12m; // Simplified monthly calc

        // 2. Check useful life limit
        if (TotalAccumulatedDepreciation + monthlyDepreciation > AcquisitionCost.Amount - Schedule.SalvageValue)
        {
            // Calculate final 'plug' amount to hit the salvage value exactly
            monthlyDepreciation = AcquisitionCost.Amount - Schedule.SalvageValue - TotalAccumulatedDepreciation;
            if (monthlyDepreciation <= 0)
            {
                // Already fully depreciated
                depreciationAmount = new Money(0, AcquisitionCost.Currency);
                return;
            }
        }
        
        depreciationAmount = new Money(monthlyDepreciation, AcquisitionCost.Currency);
        TotalAccumulatedDepreciation += monthlyDepreciation;

        // 3. Raise Domain Event for GL posting
        AddDomainEvent(new DepreciationPostedEvent(
            this.Id,
            depreciationAmount,
            periodDate,
            this.DepreciationExpenseAccountId,
            this.AccumulatedDepreciationAccountId,
            this.CostCenterId
        ));
    }
}