using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.Encumbrance.Events;

public record EncumbranceLiquidatedEvent(
    Guid EncumbranceId,
    Money Amount,
    Guid GlAccountId,
    Guid? CostCenterId,
    Guid ActualTransactionId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}