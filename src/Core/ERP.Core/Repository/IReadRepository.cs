using ERP.Core.Collections;
using ERP.Core.Entities;
using ERP.Core.Specifications;

namespace ERP.Core.Repository;

/// <summary>
/// Read-only repository interface for query operations only
/// </summary>
public interface IReadRepository<T, in TKey> where T: Entity<TKey>
{
    Task<IPagedList<T>> GetPaginationListAsync(Specification<T> specification, CancellationToken cancellationToken = default);
    Task<IPagedList<TResult>> GetPaginationListAsync<TResult>(Specification<T, TResult> specification, CancellationToken cancellationToken = default)
        where TResult : class;
    ValueTask<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> spec, CancellationToken cancellationToken = default)
        where TResult : class;
    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec, CancellationToken cancellationToken = default)
        where TResult : class;
    Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<int> CountAlAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    
    IQueryable<T> QueryableData { get; }
}

public interface IReadRepository<T> : IReadRepository<T, Guid> where T : Entity<Guid>
{
    
}