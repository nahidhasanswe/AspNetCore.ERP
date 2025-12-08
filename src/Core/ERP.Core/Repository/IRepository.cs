using ERP.Core.Entities;

namespace ERP.Core.Repository;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
public interface IRepository<T, TKey> : IReadRepository<T, TKey>, ICommandRepository<T, TKey> where T : Entity<TKey>
{
    
}

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
public interface IRepository<T> : IReadRepository<T>, ICommandRepository<T> where T : Entity<Guid>
{
    
}