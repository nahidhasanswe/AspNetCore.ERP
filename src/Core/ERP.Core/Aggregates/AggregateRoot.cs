using ERP.Core.Entities;
using ERP.Core.Events;

namespace ERP.Core.Aggregates;

public abstract class AggregateRoot<TKey>(TKey id) : Entity<TKey>(id), IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public override IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public new void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public override void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

public abstract class AggregateRoot : AggregateRoot<Guid>
{
    protected AggregateRoot() : base(Guid.NewGuid()) {}
    
    protected AggregateRoot(Guid id) : base(id)
    {
    }
}