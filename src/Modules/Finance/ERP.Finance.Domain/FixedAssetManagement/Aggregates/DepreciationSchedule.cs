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
                depreciationAmount = depreciationBase / UsefulLifeYears;
                break;

            case DepreciationMethod.DoubleDecliningBalance:
                decimal bookValue = acquisitionCost - accumulatedDepreciation;
                decimal ddbRate = (1m / UsefulLifeYears) * 2m;
                depreciationAmount = bookValue * ddbRate;
                break;
                
            case DepreciationMethod.SumOfTheYearsDigits:
                decimal sumOfYears = UsefulLifeYears * (UsefulLifeYears + 1m) / 2m;
                decimal remainingLife = UsefulLifeYears - (period - 1);
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
        if (UsefulLifeYears <= 0) return 0m;
        decimal depreciableBase = acquisitionCost - SalvageValue;
        if (depreciableBase <= 0) return 0m;

        switch (Method)
        {
            case DepreciationMethod.StraightLine:
                return depreciableBase / UsefulLifeYears;

            case DepreciationMethod.DoubleDecliningBalance:
                // For annual calculation, we need to know the current book value.
                // This method is typically called for the *first* year's full depreciation.
                // For subsequent years, CalculateDepreciationForPeriod is more appropriate.
                // For simplicity, we'll calculate the first year's DDB here.
                decimal ddbRate = (1m / UsefulLifeYears) * 2m;
                return acquisitionCost * ddbRate;

            case DepreciationMethod.SumOfTheYearsDigits:
                // For annual calculation, we need to know the current period.
                // For simplicity, we'll calculate the first year's SYD here.
                decimal sumOfYears = UsefulLifeYears * (UsefulLifeYears + 1m) / 2m;
                return depreciableBase * (UsefulLifeYears / sumOfYears);

            default:
                throw new NotSupportedException($"Depreciation method {Method} is not implemented.");
        }
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Method;
        yield return UsefulLifeYears;
        yield return SalvageValue;
    }
}