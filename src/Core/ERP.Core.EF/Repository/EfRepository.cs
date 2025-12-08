using ERP.Core.Collections;
using ERP.Core.Entities;
using ERP.Core.Repository;
using ERP.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ERP.Core.EF.Repository;

public abstract class EfRepository<TContext, T, TKey>(IDbContextProvider<TContext> dbContextProvider)
    : IReadRepository<T, TKey>, ICommandRepository<T, TKey>
    where T : Entity<TKey>
    where TContext : DbContext
{
    private readonly TContext _context = dbContextProvider.GetDbContext() ?? throw new ArgumentNullException(nameof(dbContextProvider));
    
    protected TContext Context => _context;
    
    protected DbSet<T> Table => _context.Set<T>();


    // Read

    public IQueryable<T> QueryableData => Table.AsQueryable();

    public async Task<IPagedList<T>> GetPaginationListAsync(Specification<T> specification, CancellationToken cancellationToken = default)
    {
        var result = await ApplySpecification(specification).ToListAsync(cancellationToken);
        var count = await CountAlAsync(specification, cancellationToken);
        
        return new PagedList<T>(result, count);
    }

    public async Task<IPagedList<TResult>> GetPaginationListAsync<TResult>(Specification<T, TResult> specification, CancellationToken cancellationToken = default) where TResult : class
    {
        var result = await ApplySpecification(specification).ToListAsync(cancellationToken);
        var count = await CountAlAsync(specification, cancellationToken);
        
        return new PagedList<TResult>(result, count);
    }

    public ValueTask<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return Table.FindAsync(id, cancellationToken);
    }

    public Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> spec, CancellationToken cancellationToken = default)
        where TResult : class
    {
        return ApplySpecification<TResult>(spec).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return (await Table.AsNoTracking().ToListAsync(cancellationToken)).AsReadOnly();
    }

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return (await ApplySpecification(spec).AsNoTracking().ToListAsync(cancellationToken)).AsReadOnly();
    }

    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec, CancellationToken cancellationToken = default) where TResult : class
    {
        return (await ApplySpecification(spec).AsNoTracking().ToListAsync(cancellationToken)).AsReadOnly();
    }

    public Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return ApplySpecification(spec).CountAsync(cancellationToken);
    }
    
    public Task<int> CountAlAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return Table.Where(spec.Criteria).CountAsync(cancellationToken);
    }

    public Task<bool> AnyAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return ApplySpecification(spec).AnyAsync(cancellationToken);
    }

    
    // Command
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Table.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await Table.AddRangeAsync(entities, cancellationToken);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        AttachIfNot(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var enumerable = entities as T[] ?? entities.ToArray();
        foreach (var entity in enumerable)
        {
            AttachIfNot(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        
        Table.UpdateRange(enumerable);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        Table.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        Table.RemoveRange(entities);
        return Task.CompletedTask;
    }
    
    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T, TKey>.GetQuery(Table.AsQueryable(), spec);
    }
    
    
    private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> spec)
        where TResult : class
    {
        return SpecificationEvaluator<T, TKey>.GetResultQuery(Table.AsQueryable(), spec);
    }
    protected virtual void AttachIfNot(T entity)
    {
        var entry = _context.ChangeTracker.Entries().FirstOrDefault(ent => (T)ent.Entity == entity);
        if (entry != null)
        {
            return;
        }

        Table.Attach(entity);
    }
}

public abstract class EfRepository<TContext, T>(IDbContextProvider<TContext> dbContextProvider)
    : EfRepository<TContext, T, Guid>(dbContextProvider), IReadRepository<T, Guid>, ICommandRepository<T, Guid>
    where T : Entity<Guid>
    where TContext : DbContext
{
    
}