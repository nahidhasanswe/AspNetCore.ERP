using ERP.Core.Entities;

namespace ERP.Finance.Domain.TaxManagement.Aggregates;

public class TaxRate : Entity
{
    public Guid JurisdictionId { get; private set; }
    public decimal Rate { get; private set; } // Stored as a decimal (e.g., 0.0825 for 8.25%)
    public DateTime EffectiveDate { get; private set; }
    public Guid TaxPayableAccountId { get; private set; } // The GL Account ID for the liability
    
    private TaxRate() { }

    public TaxRate(Guid jurisdictionId, decimal rate, DateTime effectiveDate, Guid payableAccountId) : base(Guid.NewGuid())
    {
        if (rate < 0 || rate > 1) throw new ArgumentException("Rate must be between 0 and 1.");
        
        JurisdictionId = jurisdictionId;
        Rate = rate;
        EffectiveDate = effectiveDate.Date;
        TaxPayableAccountId = payableAccountId;
    }
}