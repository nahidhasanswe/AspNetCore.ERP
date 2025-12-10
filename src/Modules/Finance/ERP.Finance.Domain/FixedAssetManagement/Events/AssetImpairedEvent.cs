using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.FixedAssetManagement.Events;

public record AssetImpairedEvent(
    Guid AssetId,
    Guid BusinessUnitId, 
    DateTime ImpairmentDate,
    Money ImpairmentLossAmount,
    Guid ImpairmentLossAccountId, // GL Account for Impairment Loss
    Guid AssetAccountId,
    Guid AccumulatedDepreciationAccountId,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}