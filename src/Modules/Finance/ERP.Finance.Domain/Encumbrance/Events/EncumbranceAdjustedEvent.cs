using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;

namespace ERP.Finance.Domain.Encumbrance.Events;

public record EncumbranceAdjustedEvent(
    Guid EncumbranceId,
    Money AdjustmentAmount, // Positive for increase, negative for decrease
    Guid GlAccountId,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}