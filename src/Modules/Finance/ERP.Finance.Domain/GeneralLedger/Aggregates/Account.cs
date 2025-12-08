using ERP.Core;
using ERP.Core.Entities;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

public class Account : Entity
{
    public string AccountCode { get; private set; } // Simplified for this example
    public string Name { get; private set; }
    public string Type { get; private set; } // e.g., "ASSET", "EXPENSE", "LIABILITY"
    public bool IsActive { get; private set; }
    
    // Private constructor for EF Core
    private Account() { } 
    
    public Account(string code, string name, string type) : base(Guid.NewGuid())
    {
        AccountCode = code;
        Name = name;
        Type = type;
        IsActive = true;
    }

    public Result Update(string code, string name, string type)
    {
        AccountCode = code;
        Name = name;
        Type = type;
        
        return Result.Success();
    }


    public Result DeActivate()
    {
        IsActive = false;
        return Result.Success();
    }

    public Result ReActivate()
    {
        IsActive = true;
        return Result.Success();
    }
}