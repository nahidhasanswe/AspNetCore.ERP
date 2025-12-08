namespace ERP.Core.ValueObjects;

/// <summary>
/// Base class for value objects.
/// Value objects are immutable and equality is based on their values, not identity.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Returns the components that define equality for this value object
    /// </summary>
    protected abstract IEnumerable<object> GetEqualityComponents();

    public bool Equals(ValueObject other)
    {
        if (other is null) return false;
        if (GetType() != other.GetType()) return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ValueObject);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) =>
            {
                unchecked
                {
                    return current * 23 + (obj?.GetHashCode() ?? 0);
                }
            });
    }

    public static bool operator ==(ValueObject a, ValueObject b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;

        return a.Equals(b);
    }

    public static bool operator !=(ValueObject a, ValueObject b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Creates a copy of this value object
    /// </summary>
    protected T Copy<T>() where T : ValueObject
    {
        return (T)MemberwiseClone();
    }
}