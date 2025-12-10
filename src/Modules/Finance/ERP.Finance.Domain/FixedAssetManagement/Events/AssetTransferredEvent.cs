using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.FixedAssetManagement.Events;

public record AssetTransferredEvent(
    Guid AssetId,
    Guid BusinessUnitId,
    DateTime TransferDate,
    Guid? OldCostCenterId,
    Guid? NewCostCenterId,
    Guid AssetAccountId, // Needed for GL posting
    Guid AccumulatedDepreciationAccountId, // Added this
    Money AcquisitionCost, // Needed for GL posting
    decimal TotalAccumulatedDepreciation // Needed for GL posting
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}