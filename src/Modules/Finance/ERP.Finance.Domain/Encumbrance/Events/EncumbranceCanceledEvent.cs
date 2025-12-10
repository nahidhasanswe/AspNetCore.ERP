using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.Encumbrance.Events;

public record EncumbranceCanceledEvent(
    Guid EncumbranceId,
    Guid BusinessUnitId, // New property
    Money Amount,
    Guid GlAccountId,
    Guid? CostCenterId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}