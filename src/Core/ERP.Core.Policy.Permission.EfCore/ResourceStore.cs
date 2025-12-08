using System.Diagnostics.CodeAnalysis;
using ERP.Core.Policy.Permission.Store;
using Microsoft.EntityFrameworkCore;

namespace ERP.Core.Policy.Permission.EfCore;

public class ResourceStore<TContext>(TContext context)
    : ResourceStore<string, Store.Resource<string>, Store.Permission<string>, Store.ResourcePermission<string>, TContext>(
        context)
    where TContext : DbContext;


public class ResourceStore<TKey, TContext>(TContext context)
    : ResourceStore<TKey, Store.Resource<TKey>, Store.Permission<TKey>, Store.ResourcePermission<TKey>, TContext>(
        context)
    where TKey : IEquatable<TKey>
    where TContext : DbContext;


public class ResourceStore<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TKey, TResource, TPermission, TResourcePermission, TContext> : 
    ResourceStoreBase<TKey, TResource, TPermission>,
    IQueryableResourceStore<TResource>,
    IResourceStore<TResource>,
    IResourcePermissionStore<TResource>,
    IQueryablePermissionStore<TPermission>,
    IPermissionStore<TPermission>,
    IResourceSecurityStampStore<TResource>
    where TKey: IEquatable<TKey>
    where TResource: Store.Resource<TKey>, new()
    where TPermission : Store.Permission<TKey>, new()
    where TResourcePermission : Store.ResourcePermission<TKey>, new()
    where TContext : DbContext
{
    
    private const string ConcurrencyFailure = "Optimistic concurrency failure, object has been modified.";
    
    public virtual TContext Context { get; private set; }

    public ResourceStore(TContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        Context = context;
    }

    private DbSet<TResource> ResourceSet => Context.Set<TResource>();
    private DbSet<TPermission> PermissionSet => Context.Set<TPermission>();
    private DbSet<TResourcePermission> ResourcePermissionSet => Context.Set<TResourcePermission>();
    
    public bool AutoSaveChanges { get; set; } = true;

    /// <summary>Saves the current store.</summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected Task SaveChanges(CancellationToken cancellationToken)
    {
        return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
    }

    public override IQueryable<TResource> Resources => ResourceSet.AsQueryable();
    public override async Task<Result> CreateAsync(TResource resource, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        Context.Add(resource);
        await SaveChanges(cancellationToken);
        return Result.Success();
    }

    public override async Task<Result> UpdateAsync(TResource resource, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);

        Context.Attach(resource);
        resource.ConcurrencyStamp = Guid.NewGuid().ToString();
        Context.Update(resource);
        try
        {
            await SaveChanges(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure(ConcurrencyFailure);
        }
        return Result.Success();
    }

    public override async Task<Result> DeleteAsync(TResource resource, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);

        Context.Remove(resource);
        try
        {
            await SaveChanges(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure(ConcurrencyFailure);
        }
        return Result.Success();
    }

    public override Task<TResource?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        return Resources.FirstOrDefaultAsync(x => x.Name.ToUpper() == name.ToUpper(), cancellationToken);
    }

    public override Task<TResource?> FindByIdAsync(string resourceId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var id = ConvertIdFromString(resourceId);
        return ResourceSet.FindAsync(new object?[] { id }, cancellationToken).AsTask();
    }

    public override async Task AddPermissionAsync(TResource resource, string action, string? scope,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(resource);

        var existingPermission = await PermissionSet.FirstOrDefaultAsync(x => x.Action.ToUpper() == action.ToUpper()
                                                                              && x.Scope == scope, cancellationToken);

        if (existingPermission is null)
            throw new Exception($"Permission did not exist for action: {action} and scope: {scope}");

        var create = new TResourcePermission()
        {
            ResourceId = resource.Id,
            PermissionId = existingPermission.Id
        };
        
        Context.Add(create);
        await SaveChanges(cancellationToken);
    }

    public override async Task RemovePermissionAsync(TResource resource, string action, string? scope,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        
        var existingPermission = await PermissionSet.FirstOrDefaultAsync(x => x.Action.ToUpper() == action.ToUpper()
                                                                              && x.Scope == scope, cancellationToken);
        if (existingPermission is not null)
        {
            Context.Remove(resource);
            await SaveChanges(cancellationToken);
        }
    }

    public override async Task<IList<string>> GetPermissionsAsync(string action, string? scope, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(action);
        
        var existingPermission = await PermissionSet.FirstOrDefaultAsync(x => x.Action.ToUpper() == action.ToUpper()
                                                                              && x.Scope == scope, cancellationToken);

        if (existingPermission is null)
            return new List<string>();

        var permissionId = existingPermission.Id;

        var query = from resource in ResourceSet
            join resourcePermission in ResourcePermissionSet on resource.Id equals resourcePermission.ResourceId
            where resourcePermission.PermissionId.Equals(permissionId)
            select resource.Name;

        return await query.ToListAsync(cancellationToken);
    }

    public override async Task<bool> IsInPermissionAsync(TResource resource, string action, string? scope,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(action);
        
        var existingPermission = await PermissionSet.FirstOrDefaultAsync(x => x.Action.ToUpper() == action.ToUpper()
                                                                              && x.Scope == scope, cancellationToken);
        if (existingPermission is null)
            return false;

        return await ResourcePermissionSet.AnyAsync(x =>
            x.ResourceId.Equals(resource.Id) && x.PermissionId.Equals(existingPermission.Id), cancellationToken);
    }

    public override async Task<IList<TResource>> GetResourcesInPermissionAsync(string action, string? scope, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(action);
        
        var existingPermission = await PermissionSet.FirstOrDefaultAsync(x => x.Action.ToUpper() == action.ToUpper()
                                                                              && x.Scope == scope, cancellationToken);
        if (existingPermission is null)
            return new List<TResource>();
        
        var permissionId = existingPermission.Id;
        
        var query = from resource in ResourceSet
            join resourcePermission in ResourcePermissionSet on resource.Id equals resourcePermission.ResourceId
            where resourcePermission.PermissionId.Equals(permissionId)
            select resource;
        
        return await query.ToListAsync(cancellationToken);
    }

    public override async Task<Result> CreateAsync(TPermission permission, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(permission);
        Context.Add(permission);
        await SaveChanges(cancellationToken);
        return Result.Success();
    }

    public override async Task<Result> UpdateAsync(TPermission permission, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(permission);

        Context.Attach(permission);
        permission.ConcurrencyStamp = Guid.NewGuid().ToString();
        Context.Update(permission);
        try
        {
            await SaveChanges(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure(ConcurrencyFailure);
        }
        return Result.Success();
    }

    public override async Task<Result> DeleteAsync(TPermission permission, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(permission);

        Context.Remove(permission);
        try
        {
            await SaveChanges(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure(ConcurrencyFailure);
        }
        return Result.Success();
    }

    public override Task<string> GetPermissionIdAsync(TPermission resource, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);

        return Task.FromResult(ConvertIdToString(resource.Id));
    }

    public override Task<string?> GetActionNameAsync(TPermission resource, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);

        return Task.FromResult(resource.Action);
    }

    public override Task<string?> GetScopeNameAsync(TPermission resource, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(resource);

        return Task.FromResult(resource.Scope);
    }

    public override Task<TPermission?> FindByActionAndScopeAsync(string action, string? scope, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(action);

        return PermissionSet.FirstOrDefaultAsync(x => x.Action.ToUpper() == action.ToUpper(), cancellationToken);
    }

    public override Task<TPermission?> FindPermissionByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(id);
        
        var permissionId = ConvertIdFromString(id);

        return PermissionSet.FirstOrDefaultAsync(x => x.Id.Equals(permissionId), cancellationToken);
    }

    public override IQueryable<TPermission> Permissions => PermissionSet;
}