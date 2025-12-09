using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.FixedAssetManagement.Events;

public record AssetRevaluedEvent(
    Guid AssetId,
    Guid BusinessUnitId, // Added this
    DateTime RevaluationDate,
    Money OldAcquisitionCost,
    Money NewAcquisitionCost,
    decimal OldTotalAccumulatedDepreciation,
    Guid RevaluationGainLossAccountId, // GL Account for Revaluation Gain/Loss
    Guid AssetAccountId,
    Guid AccumulatedDepreciationAccountId,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}