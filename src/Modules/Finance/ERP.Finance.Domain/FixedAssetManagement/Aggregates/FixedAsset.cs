using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.FixedAssetManagement.Enums; // Added for FixedAssetStatus
using ERP.Finance.Domain.FixedAssetManagement.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

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
    public decimal TotalAccumulatedDepreciation { get; private set; } // Tracks depreciation posted so far
    public FixedAssetStatus Status { get; private set; } // New property

    private FixedAsset() { }

    public FixedAsset(string tagNumber, string description, Money cost, DateTime date, 
                      Guid assetId, Guid expenseId, Guid accumulatedId, DepreciationSchedule schedule, Guid? costCenterId) 
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
        TotalAccumulatedDepreciation = 0m;
        Status = FixedAssetStatus.Active; // Initial status
    }

    public void Update(string description, Guid assetAccountId, Guid depreciationExpenseAccountId, Guid accumulatedDepreciationAccountId, Guid? costCenterId)
    {
        if (Status == FixedAssetStatus.Disposed || Status == FixedAssetStatus.Retired)
            throw new DomainException("Cannot update a disposed or retired asset.");
        
        Description = description;
        AssetAccountId = assetAccountId;
        DepreciationExpenseAccountId = depreciationExpenseAccountId;
        AccumulatedDepreciationAccountId = accumulatedDepreciationAccountId;
        CostCenterId = costCenterId;
        // Optionally raise an AssetUpdatedEvent
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