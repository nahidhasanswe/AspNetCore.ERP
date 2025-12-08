using System.Linq.Expressions;

namespace ERP.Core.Specifications;

/// <summary>
/// Composite specification for combining multiple specifications
/// </summary>
public abstract class CompositeSpecification<T> : Specification<T>
{
    public static Specification<T> operator &(CompositeSpecification<T> left, CompositeSpecification<T> right)
    {
        return new AndSpecification<T>(left, right);
    }

    public static Specification<T> operator |(CompositeSpecification<T> left, CompositeSpecification<T> right)
    {
        return new OrSpecification<T>(left, right);
    }

    public static Specification<T> operator !(CompositeSpecification<T> specification)
    {
        return new NotSpecification<T>(specification);
    }
}

internal sealed class AndSpecification<T> : Specification<T>
{
    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var combined = Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                Expression.Invoke(left.Criteria, param),
                Expression.Invoke(right.Criteria, param)
            ), param);

        AddCriteria(combined);
    }
}

internal sealed class OrSpecification<T> : Specification<T>
{
    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var combined = Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(
                Expression.Invoke(left.Criteria, param),
                Expression.Invoke(right.Criteria, param)
            ), param);

        AddCriteria(combined);
    }
}

internal sealed class NotSpecification<T> : Specification<T>
{
    public NotSpecification(ISpecification<T> specification)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var notExpression = Expression.Lambda<Func<T, bool>>(
            Expression.Not(Expression.Invoke(specification.Criteria, param)),
            param);

        AddCriteria(notExpression);
    }
}