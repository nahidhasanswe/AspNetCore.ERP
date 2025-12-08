using Microsoft.Extensions.Logging;

namespace ERP.Core.Policy.Permission;

public class PermissionManager<TPermission> : IDisposable
    where TPermission : class
{
    /// <summary>
    /// The cancellation token used to cancel operations.
    /// </summary>
    protected virtual CancellationToken CancellationToken => CancellationToken.None;

    private bool _disposed;

    public PermissionManager(IPermissionStore<TPermission> store, IServiceProvider serviceProvider, ILogger<PermissionManager<TPermission>> logger, IEnumerable<IPermissionValidator<TPermission>> permissionsValidators)
    {
        ArgumentNullException.ThrowIfNull(store);
        Store = store;
        Logger = logger;
        ServiceProvider = serviceProvider;

        if (permissionsValidators != null)
        {
            foreach (var validator in permissionsValidators)
            {
                Validators.Add(validator);
            }
        }
    }

    protected internal IPermissionStore<TPermission> Store { get; private set; }
    public IList<IPermissionValidator<TPermission>> Validators { get; } = new List<IPermissionValidator<TPermission>>();
  
    public ILogger Logger { get; set; }
    public IServiceProvider ServiceProvider { get; }

    public virtual bool SupportsQueryablePermissions
    {
        get
        {
            ThrowIfDisposed();
            return Store is IQueryablePermissionStore<TPermission>;
        }
    }

    public virtual IQueryable<TPermission> Permissions
    {
        get
        {
            var queryableStore = Store as IQueryablePermissionStore<TPermission>;
            if (queryableStore == null)
            {
                throw new NotSupportedException("Store does not implement IQueryablePermissionStore");
            }
            return queryableStore.Permissions;
        }
    }
    
    public virtual Task<TPermission?> FindByIdAsync(string id)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(id);
        return Store.FindPermissionByIdAsync(id, CancellationToken);
    }

    public virtual Task<TPermission?> FindByNameAsync(string action, string? scope)
    {
        ThrowIfDisposed();
        return Store.FindByActionAndScopeAsync(action, scope, CancellationToken);
    }

    public virtual async Task<Result> CreateAsync(TPermission permission)
    {
        return await CreateCoreAsync(permission).ConfigureAwait(false);
    }

    private async Task<Result> CreateCoreAsync(TPermission permission)
    {
        ThrowIfDisposed();
        var validateUserResult = await ValidatePermissionAsync(permission).ConfigureAwait(false);
        if (validateUserResult.IsFailure)
        {
            return validateUserResult;
        }

        return await Store.CreateAsync(permission, CancellationToken).ConfigureAwait(false);
    }


    public virtual async Task<Result> UpdateAsync(TPermission permission)
    {
        return await UpdatePermissionAsync(permission).ConfigureAwait(false);
    }

    protected virtual async Task<Result> UpdatePermissionAsync(TPermission permission)
    {
        ThrowIfDisposed();
        var validateUserResult = await ValidatePermissionAsync(permission).ConfigureAwait(false);
        if (validateUserResult.IsFailure)
        {
            return validateUserResult;
        }
       
        return await Store.UpdateAsync(permission, CancellationToken).ConfigureAwait(false);
    }


    public virtual async Task<Result> DeleteAsync(TPermission permission)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(permission);

        return await Store.DeleteAsync(permission, CancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<bool> PermissionExistsAsync(string action, string? scope)
    {
        ThrowIfDisposed();
        return await Store.FindByActionAndScopeAsync(action, scope, CancellationToken) is not null;
    }

    public virtual async Task<string?> GetActionNameAsync(TPermission permission)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(permission);
        return await Store.GetActionNameAsync(permission, CancellationToken);
    }
    
    public virtual async Task<string?> GetScopeNameAsync(TPermission permission)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(permission);
        return await Store.GetScopeNameAsync(permission, CancellationToken);
    }

    protected async Task<Result> ValidatePermissionAsync(TPermission permission)
    {
        List<string>? errors = null;
        foreach (var v in Validators)
        {
            var result = await v.ValidateAsync(this, permission).ConfigureAwait(false);
            if (result.IsFailure)
            {
                errors ??= new List<string>();
                errors.AddRange(result.Error);
            }
        }
        if (errors?.Count > 0)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug("Permission validation failed: {errors}.", string.Join(";", errors));
            }
            return Result.Failure(errors);
        }

        return Result.Success();
    }

    /// <summary>
    /// Throws if this class has been disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
