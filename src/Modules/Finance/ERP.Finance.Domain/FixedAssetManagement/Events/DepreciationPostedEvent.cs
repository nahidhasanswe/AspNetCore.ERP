using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.FixedAssetManagement.Events;

public record DepreciationPostedEvent(
    Guid AssetId,
    Guid BusinessUnitId, // New property
    Money Amount, 
    DateTime PeriodDate, 
    Guid DepreciationExpenseAccountId,
    Guid AccumulatedDepreciationAccountId,
    Guid? CostCenterId 
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}