using ERP.Core.Events;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record CustomerCreditHoldReleasedEvent(
    Guid CustomerId,
    Guid CreditProfileId
) : IDomainEvent 
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}