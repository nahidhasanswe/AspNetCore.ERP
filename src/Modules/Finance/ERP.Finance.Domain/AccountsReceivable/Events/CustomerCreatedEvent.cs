using ERP.Core.Events;

namespace ERP.Finance.Domain.AccountsReceivable.Events;

public record CustomerCreatedEvent(
    Guid CustomerId,
    string Name,
    string ContactEmail
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}