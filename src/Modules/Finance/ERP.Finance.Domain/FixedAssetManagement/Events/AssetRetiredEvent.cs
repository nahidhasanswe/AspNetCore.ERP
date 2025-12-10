using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.FixedAssetManagement.Events;

public record AssetRetiredEvent(
    Guid AssetId,
    Guid BusinessUnitId, // New property
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