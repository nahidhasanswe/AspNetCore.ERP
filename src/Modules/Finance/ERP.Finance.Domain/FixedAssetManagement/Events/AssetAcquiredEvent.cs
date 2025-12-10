using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.FixedAssetManagement.Events;

public record AssetAcquiredEvent(
    Guid AssetId,
    Guid BusinessUnitId,
    string TagNumber,
    string Description,
    Money AcquisitionCost,
    DateTime AcquisitionDate,
    Guid AssetAccountId, 
    Guid DepreciationExpenseAccountId,
    Guid AccumulatedDepreciationAccountId,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}