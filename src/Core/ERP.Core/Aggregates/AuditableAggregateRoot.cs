using ERP.Core.Entities;

namespace ERP.Core.Aggregates;

public abstract class AuditableAggregateRoot<T>(T id) : AggregateRoot<T>(id), IAuditableEntity
{
    public DateTime CreatedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public DateTime? LastModifiedAt { get; protected set; }
    public string? LastModifiedBy { get; protected set; }
}

public abstract class AuditableAggregateRoot: AggregateRoot<Guid>, IAuditableEntity
{
    public DateTime CreatedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public DateTime? LastModifiedAt { get; protected set; }
    public string? LastModifiedBy { get; protected set; }
    
    protected AuditableAggregateRoot() : base(Guid.NewGuid()) {}
    
    protected AuditableAggregateRoot(Guid id) : base(id)
    {
    }
}