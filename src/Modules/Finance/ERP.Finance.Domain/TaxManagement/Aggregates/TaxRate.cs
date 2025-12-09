using ERP.Core; // For Result
using ERP.Core.Entities;
using System;

namespace ERP.Finance.Domain.TaxManagement.Aggregates;

public class TaxRate : Entity
{
    public Guid JurisdictionId { get; private set; }
    public decimal Rate { get; private set; } // Stored as a decimal (e.g., 0.0825 for 8.25%)
    public DateTime EffectiveDate { get; private set; }
    public Guid TaxPayableAccountId { get; private set; } // The GL Account ID for the liability
    public bool IsActive { get; private set; } // New property
    
    private TaxRate() { }

    public TaxRate(Guid jurisdictionId, decimal rate, DateTime effectiveDate, Guid payableAccountId) : base(Guid.NewGuid())
    {
        if (rate < 0 || rate > 1) throw new ArgumentException("Rate must be between 0 and 1.");
        
        JurisdictionId = jurisdictionId;
        Rate = rate;
        EffectiveDate = effectiveDate.Date;
        TaxPayableAccountId = payableAccountId;
        IsActive = true; // New rates are active by default
    }

    public Result Update(decimal newRate, DateTime newEffectiveDate, Guid newTaxPayableAccountId)
    {
        if (!IsActive)
            return Result.Failure("Cannot update an inactive tax rate.");
        if (newRate < 0 || newRate > 1)
            return Result.Failure("Rate must be between 0 and 1.");
        
        Rate = newRate;
        EffectiveDate = newEffectiveDate.Date;
        TaxPayableAccountId = newTaxPayableAccountId;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Failure("Tax rate is already inactive.");
        IsActive = false;
        return Result.Success();
    }
}