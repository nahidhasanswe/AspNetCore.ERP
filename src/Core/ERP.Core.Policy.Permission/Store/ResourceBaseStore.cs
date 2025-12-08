using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ERP.Core.Policy.Permission.Store;

public abstract class ResourceStoreBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TKey, TResource, TPermission> :
    IQueryableResourceStore<TResource>,
    IResourceStore<TResource>,
    IResourcePermissionStore<TResource>,
    IQueryablePermissionStore<TPermission>,
    IPermissionStore<TPermission>,
    IResourceSecurityStampStore<TResource>
    where TKey: IEquatable<TKey>
    where TResource: Resource<TKey>, new()
    where TPermission : Permission<TKey>, new()
{
    private bool _disposed;

    public abstract IQueryable<TResource> Resources { get; }
    
    /// <summary>
    /// Throws if this class has been disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }

    /// <summary>
    /// Dispose the stores
    /// </summary>
    public void Dispose() => _disposed = true;


    public Task<string> GetResourceIdAsync(TResource resource, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        return Task.FromResult(ConvertIdToString(resource.Id));
    }

    public Task<string?> GetResourceNameAsync(TResource resource, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        return Task.FromResult(resource.Name);
    }

    public Task SetResourceNameAsync(TResource resource, string? name,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(name);
        resource.Name = name;
        return Task.CompletedTask;
    }

    public abstract Task<Result> CreateAsync(TResource resource, CancellationToken cancellationToken = default);

    public abstract Task<Result> UpdateAsync(TResource resource, CancellationToken cancellationToken = default);

    public abstract Task<Result> DeleteAsync(TResource resource, CancellationToken cancellationToken = default);

    public abstract Task<TResource?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    public abstract Task<TResource?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    public abstract Task AddPermissionAsync(TResource resource, string action, string? scope,
        CancellationToken cancellationToken = default);

    public abstract Task RemovePermissionAsync(TResource resource, string action, string? scope,
        CancellationToken cancellationToken = default);

    public abstract Task<IList<string>> GetPermissionsAsync(string action, string? scope,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> IsInPermissionAsync(TResource resource, string action, string? scope,
        CancellationToken cancellationToken = default);

    public abstract Task<IList<TResource>> GetResourcesInPermissionAsync(string action, string? scope,
        CancellationToken cancellationToken = default);
    
    protected virtual string ConvertIdToString(TKey id)
    {
        if (object.Equals(id, default(TKey)))
        {
            return string.Empty;
        }
        return id.ToString() ?? string.Empty;
    }
    
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "TKey is annoated with RequiresUnreferencedCodeAttribute.All.")]
    protected virtual TKey? ConvertIdFromString(string? id)
    {
        if (id == null)
        {
            return default(TKey);
        }
        return (TKey?)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
    }

    public abstract Task<Result> CreateAsync(TPermission permission, CancellationToken cancellationToken = default);

    public abstract Task<Result> UpdateAsync(TPermission permission, CancellationToken cancellationToken = default);

    public abstract Task<Result> DeleteAsync(TPermission permission, CancellationToken cancellationToken = default);

    public abstract Task<string> GetPermissionIdAsync(TPermission resource,
        CancellationToken cancellationToken = default);

    public abstract Task<string?> GetActionNameAsync(TPermission resource,
        CancellationToken cancellationToken = default);
    public abstract Task<string?> GetScopeNameAsync(TPermission resource,
        CancellationToken cancellationToken = default);

    public Task SetPermissionActionAsync(TPermission resource, string action,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(action);
        resource.Action = action;
        return Task.CompletedTask;
    }

    public Task SetPermissionScopeAsync(TPermission resource, string? scope, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        resource.Scope = scope;
        return Task.CompletedTask;
    }

    public abstract Task<TPermission?> FindByActionAndScopeAsync(string action, string? scope,
        CancellationToken cancellationToken = default);

    public abstract Task<TPermission?>
        FindPermissionByIdAsync(string id, CancellationToken cancellationToken = default);
    public abstract IQueryable<TPermission> Permissions { get; }
    
    public Task SetSecurityStampAsync(TResource resource, string stamp, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(stamp);
        resource.ConcurrencyStamp = stamp;
        return Task.CompletedTask;
    }

    public Task<string?> GetSecurityStampAsync(TResource resource, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        return Task.FromResult(resource.ConcurrencyStamp);
    }
}