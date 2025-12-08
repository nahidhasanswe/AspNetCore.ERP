using ERP.Core.Events;

namespace ERP.Core.Aggregates;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}