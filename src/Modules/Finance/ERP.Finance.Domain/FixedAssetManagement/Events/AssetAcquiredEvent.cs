using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.FixedAssetManagement.Events;

public record AssetAcquiredEvent(
    Guid AssetId,
    string TagNumber,
    Money AcquisitionCost,
    DateTime AcquisitionDate,
    Guid AssetAccountId, // GL Account (e.g., Computer Equipment)
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}