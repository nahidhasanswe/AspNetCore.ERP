using ERP.Core; // For Result
using ERP.Core.Entities;
using System;

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

    public Result Update(string newName, string newRegionCode)
    {
        if (!IsActive)
            return Result.Failure("Cannot update an inactive tax jurisdiction.");
        
        Name = newName;
        RegionCode = newRegionCode;
        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive)
            return Result.Failure("Tax jurisdiction is already active.");
        IsActive = true;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Failure("Tax jurisdiction is already inactive.");
        IsActive = false;
        return Result.Success();
    }
}