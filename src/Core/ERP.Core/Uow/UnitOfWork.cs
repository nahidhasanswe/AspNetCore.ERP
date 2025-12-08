namespace ERP.Core.Uow;

public abstract class UnitOfWork : IUnitOfWork
{
    private bool disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        disposed = true;         
    }

    /// <inheritdoc/>
    public abstract Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}