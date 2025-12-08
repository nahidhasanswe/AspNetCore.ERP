using ERP.Core.Entities;
using ERP.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ERP.Core.EF;

/// <summary>
/// Evaluates a specification and applies its criteria to an IQueryable.
/// </summary>
public static class SpecificationEvaluator<T, TKey> where T:  Entity<TKey>
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery;

        // 1. Filtering criteria
        query = query.Where(specification.Criteria);

        // 2. Includes (Eager Loading)
        query = specification.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        query = specification.IncludeStrings.Aggregate(query,
            (current, include) => current.Include(include));
        

        if (specification?.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }
        else if (specification?.OrderBy is not null)
        {
            // 3. Ordering
            query = query.OrderBy(specification.OrderBy);
        }

        // 4. Grouping
        if (specification?.GroupBy is not null)
        {
            query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
        }
        

        // 5. Paging
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        // 6. AsNoTracking
        if (specification.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    public static IQueryable<TResult> GetResultQuery<TResult>(IQueryable<T> inputQuery, ISpecification<T, TResult> specification)
        where TResult: class
    {
        var query = GetQuery(inputQuery, specification);
        return query.Select(specification.Selector);
    }
}

public static class SpecificationEvaluator<T> where T:  Entity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        return SpecificationEvaluator<T, Guid>.GetQuery(inputQuery, specification);
    }
    
    public static IQueryable<TResult> GetResultQuery<TResult>(IQueryable<T> inputQuery, ISpecification<T, TResult> specification)
        where TResult : class
    {
        return SpecificationEvaluator<T, Guid>.GetResultQuery(inputQuery, specification);
    }
}