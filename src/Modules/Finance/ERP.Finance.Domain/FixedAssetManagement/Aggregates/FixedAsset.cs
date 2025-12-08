using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
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
    public decimal TotalAccumulatedDepreciation { get; private set; } // Tracks depreciation posted so far

    private FixedAsset() { }

    public FixedAsset(string tagNumber, string description, Money cost, DateTime date, 
                      Guid assetId, Guid expenseId, Guid accumulatedId, DepreciationSchedule schedule) 
                      : base(Guid.NewGuid())
    {
        // ... validation checks ...
        TagNumber = tagNumber;
        Description = description;
        AcquisitionCost = cost;
        AcquisitionDate = date.Date;
        AssetAccountId = assetId;
        DepreciationExpenseAccountId = expenseId;
        AccumulatedDepreciationAccountId = accumulatedId;
        Schedule = schedule;
        TotalAccumulatedDepreciation = 0m;
    }

    /// <summary>
    /// Core Domain Operation: Calculates and records depreciation for a period.
    /// </summary>
    public void Depreciate(DateTime periodDate, out Money depreciationAmount)
    {
        if (periodDate < AcquisitionDate) throw new DomainException("Cannot depreciate before acquisition date.");
        
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