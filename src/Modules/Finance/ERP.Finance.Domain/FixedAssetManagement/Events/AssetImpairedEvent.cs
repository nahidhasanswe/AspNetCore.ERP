using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.FixedAssetManagement.Events;

public record AssetImpairedEvent(
    Guid AssetId,
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