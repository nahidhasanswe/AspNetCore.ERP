using ERP.Core.Aggregates;
using ERP.Core.Entities;
using ERP.Core.Events;
using ERP.Core.Uow;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ERP.Core.EF.Uow;

/// <summary>
/// Implements Unit of work for Entity Framework.
/// </summary>
public class EfCoreUnitOfWork<TDbContext> : UnitOfWork
    where TDbContext : DbContext
{
    private bool _disposed;
    private readonly TDbContext _context;
    
    private IDbContextTransaction? _transaction;
    
    private readonly IMediator _mediator;
    
    /// <summary>
    /// Creates a new <see cref="EfCoreUnitOfWork{TDbContext}"/>.
    /// </summary>
    public EfCoreUnitOfWork(TDbContext dbContext, IMediator mediator)
    {
        _context = dbContext;
        _mediator = mediator;
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _disposed = true;

        base.Dispose(disposing);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps
        var entries = _context.ChangeTracker.Entries<Entity>()
            .Where(e => e.State == EntityState.Modified);

        // foreach (var entry in entries)
        // {
        //     entry.Entity.UpdatedAt = DateTime.UtcNow;
        // }

        var dispatchEntries = ExtractDomainEvents();
        
        var result = await _context.SaveChangesAsync(cancellationToken);

        await DispatchDomainEvents(dispatchEntries);

        return result;
    }

    public override Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public override async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _transaction?.CommitAsync(cancellationToken)!;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public override async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _transaction?.RollbackAsync(cancellationToken)!;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }
    
    private List<IDomainEvent> ExtractDomainEvents()
    {
        var domainEntities = _context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is IAggregateRoot)
            .Select(e => (IAggregateRoot)e.Entity)
            .Where(x => x.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        // Clear events immediately
        domainEntities.ForEach(entity => entity.ClearDomainEvents());
    
        return domainEvents;
    }

    private async Task DispatchDomainEvents(List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
    }
}