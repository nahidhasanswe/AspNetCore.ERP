using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.Encumbrance.Events;

public record EncumbranceAdjustedEvent(
    Guid EncumbranceId,
    Guid BusinessUnitId, // New property
    Money AdjustmentAmount, // Positive for increase, negative for decrease
    Guid GlAccountId,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}