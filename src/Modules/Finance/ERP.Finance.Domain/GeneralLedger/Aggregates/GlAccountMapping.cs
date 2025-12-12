using ERP.Core.Entities;
using System;

namespace ERP.Finance.Domain.GeneralLedger.Aggregates;

public enum GlAccountMappingType
{
    APControl,
    ARControl,
    TaxPayable,
    UnappliedCash,
    PaymentClearing, // For payment methods
    // ... other types as needed
}

public class GlAccountMapping : Entity
{
    public Guid BusinessUnitId { get; private set; }
    public GlAccountMappingType MappingType { get; private set; }
    public string Currency { get; private set; } // The currency this mapping applies to
    public Guid? ReferenceId { get; private set; } // e.g., JurisdictionId for TaxPayable, or null for general controls
    public Guid GlAccountId { get; private set; } // The target GL Account

    private GlAccountMapping() { }

    public GlAccountMapping(Guid businessUnitId, GlAccountMappingType mappingType, string currency, Guid glAccountId, Guid? referenceId = null) 
        : base(Guid.NewGuid())
    {
        BusinessUnitId = businessUnitId;
        MappingType = mappingType;
        Currency = currency;
        GlAccountId = glAccountId;
        ReferenceId = referenceId;
    }

    public void Update(Guid glAccountId, Guid? referenceId = null)
    {
        GlAccountId = glAccountId;
        ReferenceId = referenceId;
    }
}