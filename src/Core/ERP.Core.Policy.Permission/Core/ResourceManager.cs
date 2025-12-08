using Microsoft.Extensions.Logging;
namespace ERP.Core.Policy.Permission;

public class ResourceManager<TResource> : IDisposable
    where TResource : class
{

     /// <summary>
    /// The cancellation token used to cancel operations.
    /// </summary>
    protected virtual CancellationToken CancellationToken => CancellationToken.None;

    private bool _disposed;

    public ResourceManager(IResourceStore<TResource> store, IServiceProvider serviceProvider, ILogger<ResourceManager<TResource>> logger, IEnumerable<IResourceValidator<TResource>>? resourceValidators)
    {
        ArgumentNullException.ThrowIfNull(store);
        Store = store;
        Logger = logger;
        ServiceProvider = serviceProvider;

        if (resourceValidators is not null)
        {
            foreach (var validator in resourceValidators)
            {
                Validators.Add(validator);
            }
        }
    }

    protected internal IResourceStore<TResource> Store { get; set; }
    public IList<IResourceValidator<TResource>> Validators { get; } = new List<IResourceValidator<TResource>>();
  
    public ILogger Logger { get; set; }
    public IServiceProvider ServiceProvider { get; }


    public virtual bool SupportsQueryableResources
    {
        get
        {
            ThrowIfDisposed();
            return Store is IQueryableResourceStore<TResource>;
        }
    }
    
    public virtual bool SupportsUserSecurityStamp
    {
        get
        {
            ThrowIfDisposed();
            return Store is IResourceSecurityStampStore<TResource>;
        }
    }

    public virtual IQueryable<TResource> Resources
    {
        get
        {
            var queryableStore = Store as IQueryableResourceStore<TResource>;
            if (queryableStore == null)
            {
                throw new NotSupportedException("Store does not implement IQueryableResourceStore");
            }
            return queryableStore.Resources;
        }
    }
    
    public virtual Task<string> GenerateConcurrencyStampAsync(TResource resource)
    {
        return Task.FromResult(Guid.NewGuid().ToString());
    }

    public virtual async Task<Result> CreateAsync(TResource resource, CancellationToken cancellationToken = default)
    {
        return await CreateCoreAsync(resource).ConfigureAwait(false);
    }

    private async Task<Result> CreateCoreAsync(TResource resource)
    {
        ThrowIfDisposed();
        await UpdateSecurityStampInternal(resource).ConfigureAwait(false);
        var validateUserResult = await ValidateResourceAsync(resource).ConfigureAwait(false);
        if (validateUserResult.IsFailure)
        {
            return validateUserResult;
        }

        return await Store.CreateAsync(resource, CancellationToken).ConfigureAwait(false);
    }


    public virtual async Task<Result> UpdateAsync(TResource resource)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        return await UpdateResourceAsync(resource).ConfigureAwait(false);
    }

    protected virtual async Task<Result> UpdateResourceAsync(TResource resource)
    {
        ThrowIfDisposed();
        var validateUserResult = await ValidateResourceAsync(resource).ConfigureAwait(false);
        if (validateUserResult.IsFailure)
        {
            return validateUserResult;
        }
       
        return await Store.UpdateAsync(resource, CancellationToken).ConfigureAwait(false);
    }


    public virtual async Task<Result> DeleteAsync(TResource resource)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        return await Store.DeleteAsync(resource, CancellationToken).ConfigureAwait(false);
    }

    public virtual Task<TResource?> FindByIdAsync(string id)
    {
        ThrowIfDisposed();
        return Store.FindByIdAsync(id, CancellationToken);
    }

    public virtual Task<TResource?> FindByNameAsync(string name)
    {
        ThrowIfDisposed();
        return Store.FindByNameAsync(name, CancellationToken);
    }

    public virtual Task<string?> GetResourceNameAsync(TResource resource)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        return Store.GetResourceNameAsync(resource, CancellationToken);
    }
    
    public virtual Task<string> GetResourceIdAsync(TResource resource)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        return Store.GetResourceIdAsync(resource, CancellationToken);
    }
    
    public virtual async Task<string> GetSecurityStampAsync(TResource resource)
    {
        ThrowIfDisposed();
        var securityStore = GetSecurityStore();
        ArgumentNullException.ThrowIfNull(resource);
        var stamp = await securityStore.GetSecurityStampAsync(resource, CancellationToken).ConfigureAwait(false);
        if (stamp == null)
        {
            Logger.LogDebug("GetSecurityStampAsync for resource failed because stamp was null.");
            throw new InvalidOperationException("Resource security stamp is null");
        }
        return stamp;
    }
    
    public virtual async Task<Result> UpdateSecurityStampAsync(TResource resource)
    {
        ThrowIfDisposed();
        GetSecurityStore();
        ArgumentNullException.ThrowIfNull(resource);

        await UpdateSecurityStampInternal(resource).ConfigureAwait(false);
        return Result.Success();
    }
    
    public virtual async Task<Result> AddToPermissionAsync(TResource resource, string action, string? scope)
    {
        return await AddToPermissionCoreAsync(resource, action, scope).ConfigureAwait(false);
    }
    
    private async Task<Result> AddToPermissionCoreAsync(TResource resource, string action, string? scope)
    {
        ThrowIfDisposed();
        var resourcePermissionStore = GetResourcePermissionStore();
        ArgumentNullException.ThrowIfNull(resource);

        if (await resourcePermissionStore.IsInPermissionAsync(resource, action, scope, CancellationToken).ConfigureAwait(false))
        {
            return Result.Failure("Already have this permission into this resource");
        }
        await resourcePermissionStore.AddPermissionAsync(resource, action, scope, CancellationToken).ConfigureAwait(false);
        return await UpdateAsync(resource).ConfigureAwait(false);
    }
    
    public virtual async Task<Result> AddToPermissionsAsync(TResource resource, List<Models.Permission> permissions)
    {
        return await AddToPermissionsCoreAsync(resource, permissions).ConfigureAwait(false);
    }

    private async Task<Result> AddToPermissionsCoreAsync(TResource resource, List<Models.Permission> permissions)
    {
        ThrowIfDisposed();
        var resourcePermissionStore = GetResourcePermissionStore();
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(permissions);

        foreach (var permission in permissions.Distinct())
        {
            if (await resourcePermissionStore.IsInPermissionAsync(resource, permission.Action, permission.Scope, CancellationToken).ConfigureAwait(false))
            {
                return Result.Failure("Already have this permission into this resource");
            }
            await resourcePermissionStore.AddPermissionAsync(resource, permission.Action, permission.Scope, CancellationToken).ConfigureAwait(false);
        }
        return await UpdateAsync(resource).ConfigureAwait(false);
    }

    public virtual async Task<Result> RemoveFromPermissionAsync(TResource resource, string action, string? scope)
    {
        ThrowIfDisposed();
        var resourcePermissionStore = GetResourcePermissionStore();
        ArgumentNullException.ThrowIfNull(resource);

        if (!await resourcePermissionStore.IsInPermissionAsync(resource, action, scope, CancellationToken))
        {
            return Result.Failure($"Resource have no permission for action: {action}, scope: {scope}");
        }

        await resourcePermissionStore.RemovePermissionAsync(resource, action, scope, CancellationToken);
        return await UpdateAsync(resource);
    }
    
    public virtual async Task<Result> RemoveFromPermissionsAsync(TResource resource, List<Models.Permission> permissions)
    {
        ThrowIfDisposed();
        var resourcePermissionStore = GetResourcePermissionStore();
        ArgumentNullException.ThrowIfNull(resource);

        foreach (var permission in permissions)
        {
            if (!await resourcePermissionStore.IsInPermissionAsync(resource, permission.Action, permission.Scope, CancellationToken))
            {
                return Result.Failure($"Resource have no permission for action: {permission.Action}, scope: {permission.Scope}");
            }

            await resourcePermissionStore.RemovePermissionAsync(resource, permission.Action, permission.Scope, CancellationToken);
        }
        
        return await UpdateAsync(resource);
    }

    public virtual Task<IList<string>> GetResourcesFromPermissionAsync(string action, string? scope)
    {
        ThrowIfDisposed();
        var resourcePermissionStore = GetResourcePermissionStore();
        return resourcePermissionStore.GetPermissionsAsync(action, scope, CancellationToken);
    }

    public virtual Task<IList<TResource>> GetResourcesInPermissions(string action, string? scope)
    {
        ThrowIfDisposed();
        var resourcePermissionStore = GetResourcePermissionStore();
        return resourcePermissionStore.GetResourcesInPermissionAsync(action, scope, CancellationToken);
    }

    public virtual async Task<bool> IsInPermission(TResource resource, string action, string? scope)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        var resourcePermissionStore = GetResourcePermissionStore();
        return await resourcePermissionStore.IsInPermissionAsync(resource, action, scope, CancellationToken);
    }

    private async Task UpdateSecurityStampInternal(TResource resource)
    {
        if (SupportsUserSecurityStamp)
        {
            await GetSecurityStore().SetSecurityStampAsync(resource, NewSecurityStamp(), CancellationToken).ConfigureAwait(false);
        }
    }
    
    private static string NewSecurityStamp()
    {
#if NETSTANDARD2_0 || NETFRAMEWORK
        byte[] bytes = new byte[20];
        _rng.GetBytes(bytes);
        return Base32.ToBase32(bytes);
#else
        return Base32.GenerateBase32();
#endif
    }
    
    private IResourceSecurityStampStore<TResource> GetSecurityStore()
    {
        var cast = Store as IResourceSecurityStampStore<TResource>;
        if (cast == null)
        {
            throw new NotSupportedException("Store does not implement IResourceSecurityStampStore");
        }
        return cast;
    }
    
    private IResourcePermissionStore<TResource> GetResourcePermissionStore()
    {
        var cast = Store as IResourcePermissionStore<TResource>;
        if (cast == null)
        {
            throw new NotSupportedException("Store does not implement IResourcePermissionStore");
        }
        return cast;
    }


    protected async Task<Result> ValidateResourceAsync(TResource resource)
    {
        List<string>? errors = null;
        foreach (var v in Validators)
        {
            var result = await v.ValidateAsync(this, resource).ConfigureAwait(false);
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
                Logger.LogDebug("Resource validation failed: {errors}.", string.Join(";", errors));
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
