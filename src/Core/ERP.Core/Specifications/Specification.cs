using System.Linq.Expressions;

namespace ERP.Core.Specifications;


/// <summary>
/// Base class for specifications
/// </summary>
public abstract class Specification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>> OrderBy { get; private set; }
    public Expression<Func<T, object>> OrderByDescending { get; private set; }
    public Expression<Func<T, object>> GroupBy { get; private set; }

    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }
    public bool AsNoTracking { get; private set; }

    public Specification()
    {
        
    }

    public Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescending = orderByDescExpression;
    }
    
    public void ApplyOrderBy(Expression<Func<T, object>> orderByExpression, bool descending)
    {
        if (descending)
        {
            OrderByDescending = orderByExpression;
        }
        else
        {
            OrderBy = orderByExpression;
        }
    }

    protected void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
    {
        GroupBy = groupByExpression;
    }

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    protected void ApplyNoTracking()
    {
        AsNoTracking = true;
    }
}

public abstract class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult>
    where TResult : class
{
    public Expression<Func<T, TResult>> Selector { get; private set;  }


    protected void AddSelector(Expression<Func<T, TResult>> selector)
    {
        Selector = selector;
    }
}