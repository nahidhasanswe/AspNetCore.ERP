using ERP.Core.Entities;

namespace ERP.Core.Repository;

/// <summary>
/// command repository interface for write operations only
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TKey"></typeparam>
public interface ICommandRepository<T, TKey> where T: Entity<TKey>
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICommandRepository<T> : ICommandRepository<T, Guid> where T : Entity<Guid>
{
    
}