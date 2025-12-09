using ERP.Core;
using ERP.Core.Entities;
using ERP.Finance.Domain.GeneralLedger.Enums;
using System;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

public class Account : Entity
{
    public Guid BusinessUnitId { get; private set; } // New property
    public string AccountCode { get; private set; }
    public string Name { get; private set; }
    public AccountType Type { get; private set; }
    public Guid? ParentId { get; private set; } // For hierarchy
    public bool IsSummary { get; private set; } // True if it's a roll-up account that cannot be posted to.
    public bool IsActive { get; private set; }
    
    // Private constructor for EF Core
    private Account() { } 
    
    public Account(Guid businessUnitId, string code, string name, AccountType type, Guid? parentId = null, bool isSummary = false) : base(Guid.NewGuid())
    {
        BusinessUnitId = businessUnitId;
        AccountCode = code;
        Name = name;
        Type = type;
        ParentId = parentId;
        IsSummary = isSummary;
        IsActive = true;
    }

    public Result Update(string code, string name, AccountType type, Guid? parentId, Guid businessUnitId)
    {
        // Add validation logic as needed, e.g., prevent circular references for ParentId
        AccountCode = code;
        Name = name;
        Type = type;
        ParentId = parentId;
        BusinessUnitId = businessUnitId; // Allow updating BusinessUnitId
        
        return Result.Success();
    }

    public void SetAsSummary(bool isSummary)
    {
        IsSummary = isSummary;
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