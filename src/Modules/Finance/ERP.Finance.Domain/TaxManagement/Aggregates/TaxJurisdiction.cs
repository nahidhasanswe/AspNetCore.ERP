using ERP.Core.Entities;

namespace ERP.Finance.Domain.TaxManagement.Aggregates;

public class TaxJurisdiction : Entity
{
    public string Name { get; private set; }
    public string RegionCode { get; private set; } // e.g., "NY", "CA"
    public bool IsActive { get; private set; }
    
    private TaxJurisdiction() { }

    public TaxJurisdiction(string name, string regionCode) : base(Guid.NewGuid())
    {
        Name = name;
        RegionCode = regionCode;
        IsActive = true;
    }
}