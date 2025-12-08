using ERP.Core.Events;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record CustomerCreditHoldPlacedEvent(
    Guid CustomerId,
    Guid CreditProfileId,
    string Reason // e.g., "Limit Exceeded," "90 Days Past Due"
) : IDomainEvent 
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}