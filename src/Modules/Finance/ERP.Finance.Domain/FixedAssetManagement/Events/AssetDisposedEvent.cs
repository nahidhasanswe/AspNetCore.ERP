using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.FixedAssetManagement.Events;

public record AssetDisposedEvent(
    Guid AssetId,
    Guid BusinessUnitId, // New property
    DateTime DisposalDate,
    Money AcquisitionCost,
    decimal TotalAccumulatedDepreciation,
    Money Proceeds,
    decimal GainLossAmount, // Positive for gain, negative for loss
    Guid AssetAccountId,
    Guid AccumulatedDepreciationAccountId,
    Guid GainLossAccountId, // GL Account for Gain/Loss on Disposal
    Guid? CostCenterId
)  : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}