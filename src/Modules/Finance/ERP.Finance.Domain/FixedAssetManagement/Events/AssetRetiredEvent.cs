using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.FixedAssetManagement.Events;

public record AssetRetiredEvent(
    Guid AssetId,
    DateTime RetirementDate,
    Money AcquisitionCost,
    decimal TotalAccumulatedDepreciation,
    Guid AssetAccountId,
    Guid AccumulatedDepreciationAccountId,
    string Reason,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}