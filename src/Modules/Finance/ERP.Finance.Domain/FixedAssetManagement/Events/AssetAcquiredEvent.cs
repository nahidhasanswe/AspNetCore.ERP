using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.FixedAssetManagement.Events;

public record AssetAcquiredEvent(
    Guid AssetId,
    Guid BusinessUnitId, // New property
    string TagNumber,
    string Description, // Added for clarity in event
    Money AcquisitionCost,
    DateTime AcquisitionDate,
    Guid AssetAccountId, // GL Account (e.g., Computer Equipment)
    Guid DepreciationExpenseAccountId, // Added for clarity in event
    Guid AccumulatedDepreciationAccountId, // Added for clarity in event
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}