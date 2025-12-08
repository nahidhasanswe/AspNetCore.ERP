using System.Linq.Expressions;

namespace ERP.Core.Specifications;

public interface ISpecification<TEntity, TResult> : ISpecification<TEntity>
{
    Expression<Func<TEntity, TResult>> Selector { get; }
}

/// <summary>
/// Base specification interface for querying entities
/// </summary>
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>> OrderBy { get; }
    Expression<Func<T, object>> OrderByDescending { get; }
    Expression<Func<T, object>> GroupBy { get; }
        
    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
    bool AsNoTracking { get; }
}
