using ERP.Core.ValueObjects;
using ERP.Finance.Domain.Shared.Enums;

namespace ERP.Finance.Domain.FixedAssetManagement.Aggregates;

public class DepreciationSchedule : ValueObject
{
    public DepreciationMethod Method { get; private set; }
    public int UsefulLifeYears { get; private set; }
    public decimal SalvageValue { get; private set; }
    
    private DepreciationSchedule() { }
    
    public DepreciationSchedule(DepreciationMethod method, int usefulLife, decimal salvageValue)
    {
        if (usefulLife <= 0) throw new ArgumentException("Useful life must be positive.", nameof(usefulLife));
        if (salvageValue < 0) throw new ArgumentException("Salvage value cannot be negative.", nameof(salvageValue));
        
        Method = method;
        UsefulLifeYears = usefulLife;
        SalvageValue = salvageValue;
    }
    
    /// <summary>
    /// Calculates the depreciation amount for a specific period (typically one year), 
    /// ensuring the total accumulated amount does not drop below the salvage value.
    /// </summary>
    /// <param name="acquisitionCost">The initial cost of the asset.</param>
    /// <param name="period">The current period number (e.g., 1 for the first year).</param>
    /// <param name="accumulatedDepreciation">Total depreciation recognized prior to this period.</param>
    /// <returns>The calculated depreciation amount for the given period.</returns>
    public decimal CalculateDepreciationForPeriod(decimal acquisitionCost, int period, decimal accumulatedDepreciation)
    {
        if (period <= 0) return 0m;
        
        decimal depreciationBase = acquisitionCost - SalvageValue;
        if (depreciationBase <= 0) return 0m;

        decimal depreciationAmount;
        
        switch (Method)
        {
            case DepreciationMethod.StraightLine:
                // Annual Depreciation = (Cost - Salvage) / Useful Life
                depreciationAmount = depreciationBase / UsefulLifeYears;
                break;

            case DepreciationMethod.DoubleDecliningBalance:
                // Book Value = Cost - Accumulated Depreciation
                decimal bookValue = acquisitionCost - accumulatedDepreciation;
                
                // Rate = (1 / Useful Life) * 2
                decimal ddbRate = (1m / UsefulLifeYears) * 2m;
                
                // Depreciation = Book Value * Rate
                depreciationAmount = bookValue * ddbRate;
                break;
                
            case DepreciationMethod.SumOfTheYearsDigits:
                // Sum of Years = n * (n + 1) / 2
                decimal sumOfYears = UsefulLifeYears * (UsefulLifeYears + 1m) / 2m;
                
                // Remaining Life = Useful Life - (Current Period - 1)
                decimal remainingLife = UsefulLifeYears - (period - 1);
                
                // Depreciation = (Cost - Salvage) * (Remaining Life / Sum of Years)
                depreciationAmount = depreciationBase * (remainingLife / sumOfYears);
                break;
                
            default:
                throw new NotSupportedException($"Depreciation method {Method} is not implemented.");
        }
        
        // Final Check: Ensure depreciation does not push Net Book Value below Salvage Value
        decimal maxDepreciationAllowed = acquisitionCost - SalvageValue - accumulatedDepreciation;
        
        return Math.Max(0m, Math.Min(depreciationAmount, maxDepreciationAllowed));
    }
    
    public decimal CalculateAnnualDepreciation(decimal acquisitionCost)
    {
        if (Method == DepreciationMethod.StraightLine)
        {
            if (UsefulLifeYears <= 0) return 0m;
            
            decimal depreciableBase = acquisitionCost - SalvageValue;
            if (depreciableBase <= 0) return 0m;
            
            return depreciableBase / UsefulLifeYears;
        }
        
        // Add logic for other methods (DD, SYD, etc.) here
        throw new NotImplementedException($"Depreciation method {Method} is not yet implemented.");
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Method;
        yield return UsefulLifeYears;
        yield return SalvageValue; // Salvage value is critical to the schedule and should be part of equality
    }
}