using ERP.Core.Events;

namespace ERP.Core.Entities;

/// <summary>
/// Base class for all entities in the domain.
/// Entities have a unique identity (GUID) that runs through time and different representations.
/// </summary>
public abstract class Entity : Entity<Guid>
{

	protected Entity() : base(Guid.NewGuid())
	{
		
	}
	
	protected Entity(Guid id) : base(id)
	{
	}
}

/// <summary>
/// Base class for all entities in the domain.
/// Entities have a unique identity that runs through time and different representations.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
{
	public TId Id { get; protected set; }

	private List<IDomainEvent> _domainEvents = new();
	public virtual IReadOnlyCollection<IDomainEvent>? DomainEvents => _domainEvents?.AsReadOnly();

	private Entity()
	{
	}

	protected Entity(TId id)
	{
		if (Equals(id, default(TId)))
		{
			throw new ArgumentException("The ID cannot be the default value.", nameof(id));
		}

		Id = id;
	}

	/// <summary>
	/// Adds a domain event to the entity
	/// </summary>
	protected void AddDomainEvent(IDomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}

	/// <summary>
	/// Clears all domain events
	/// </summary>
	public virtual void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}

	/// <summary>
	/// Equality based on Id
	/// </summary>
	public bool Equals(Entity<TId> other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		if (GetType() != other.GetType()) return false;
		if (Equals(Id, default(TId)) || Equals(other.Id, default(TId))) return false;

		return EqualityComparer<TId>.Default.Equals(Id, other.Id);
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as Entity<TId>);
	}

	public override int GetHashCode()
	{
		return (GetType().ToString() + Id).GetHashCode();
	}

	public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
	{
		if (a is null && b is null) return true;
		if (a is null || b is null) return false;

		return a.Equals(b);
	}

	public static bool operator !=(Entity<TId> a, Entity<TId> b)
	{
		return !(a == b);
	}
}