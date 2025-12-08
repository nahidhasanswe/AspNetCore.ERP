using ERP.Core.Events;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Domain.Encumbrance.Events;

public record EncumbranceConvertedToCommitmentEvent(
    Guid EncumbranceId,
    Money FinalCommittedAmount, // The potentially new, firm amount
    Guid GlAccountId,
    Guid? CostCenterId // Mandatory dimension
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}