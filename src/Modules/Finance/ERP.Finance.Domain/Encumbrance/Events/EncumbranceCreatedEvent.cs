using ERP.Core.Events;
using ERP.Finance.Domain.Encumbrance.Enums;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.Encumbrance.Events;

public record EncumbranceCreatedEvent(
    Guid EncumbranceId,
    Guid SourceTransactionId,
    Money Amount,
    Guid GlAccountId,
    Guid? CostCenterId,
    EncumbranceType Type // Reserved or Committed
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}