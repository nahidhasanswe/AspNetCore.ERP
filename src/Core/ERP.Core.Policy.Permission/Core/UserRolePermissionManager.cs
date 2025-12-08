using Microsoft.Extensions.Logging;

namespace ERP.Core.Policy.Permission;

public class UserRolePermissionManager<TUserPermission, TRolePermission, TResourcePermission> : IDisposable
    where TUserPermission : class
    where TRolePermission : class
    where TResourcePermission : class
{

    /// <summary>
    /// The cancellation token used to cancel operations.
    /// </summary>
    protected virtual CancellationToken CancellationToken => CancellationToken.None;

    private bool _disposed;

    public UserRolePermissionManager(
        IUserRolePermissionStore<TResourcePermission, TUserPermission, TRolePermission> store, 
        IServiceProvider serviceProvider, 
        ILogger<UserRolePermissionManager<TUserPermission, TRolePermission, TResourcePermission>> logger, 
        IEnumerable<IUserPermissionValidator<TUserPermission, TRolePermission, TResourcePermission>> userPermissionsValidators,
        IEnumerable<IRolePermissionValidator<TUserPermission, TRolePermission, TResourcePermission>> rolePermissionsValidators)
    {
        ArgumentNullException.ThrowIfNull(store);
        Store = store;
        Logger = logger;
        ServiceProvider = serviceProvider;

        if (userPermissionsValidators != null)
        {
            foreach (var validator in userPermissionsValidators)
            {
                UserPermissionValidators.Add(validator);
            }
        }
        
        if (rolePermissionsValidators != null)
        {
            foreach (var validator in rolePermissionsValidators)
            {
                RolePermissionValidators.Add(validator);
            }
        }
    }

    protected internal IUserRolePermissionStore<TResourcePermission, TUserPermission, TRolePermission> Store { get; set; }
    public IList<IUserPermissionValidator<TUserPermission, TRolePermission, TResourcePermission>> UserPermissionValidators { get; } = new List<IUserPermissionValidator<TUserPermission, TRolePermission, TResourcePermission>>();
    public IList<IRolePermissionValidator<TUserPermission, TRolePermission, TResourcePermission>> RolePermissionValidators { get; } = new List<IRolePermissionValidator<TUserPermission, TRolePermission, TResourcePermission>>();
  
    public ILogger Logger { get; set; }
    public IServiceProvider ServiceProvider { get; }

    public virtual bool SupportsQueryableRolePermissions
    {
        get
        {
            ThrowIfDisposed();
            return Store is IQueryableRolePermissionStore<TRolePermission, TResourcePermission>;
        }
    }

    public virtual IQueryable<TRolePermission> RolePermissions
    {
        get
        {
            var queryableStore = Store as IQueryableRolePermissionStore<TRolePermission, TResourcePermission>;
            if (queryableStore == null)
            {
                throw new NotSupportedException("Store does not implement IQueryableRolePermissionStore");
            }
            return queryableStore.RolePermissions;
        }
    }
    
    public virtual bool SupportsQueryableUserPermissions
    {
        get
        {
            ThrowIfDisposed();
            return Store is IQueryableUserPermissionStore<TUserPermission, TResourcePermission>;
        }
    }

    public virtual IQueryable<TUserPermission> UserPermissions
    {
        get
        {
            var queryableStore = Store as IQueryableUserPermissionStore<TUserPermission, TResourcePermission>;
            if (queryableStore == null)
            {
                throw new NotSupportedException("Store does not implement IQueryableUserPermissionStore");
            }
            return queryableStore.UserPermissions;
        }
        
    }

    public virtual async Task<Result> CreateUserPermissionAsync(string userId, TResourcePermission permission)
        => await CreateUserPermissionCoreAsync(userId, permission).ConfigureAwait(false);

    private async Task<Result> CreateUserPermissionCoreAsync(string userId, TResourcePermission permission)
    {
        ThrowIfDisposed();
        var userPermissionStore = GetUserPermissionStore();
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(userPermissionStore);
        ArgumentNullException.ThrowIfNull(permission);
        var validateUserResult = await ValidateUserPermissionAsync(permission).ConfigureAwait(false);
        if (validateUserResult.IsFailure)
        {
            return validateUserResult;
        }
        
        var exist = await userPermissionStore.IsUserInPermissionAsync(userId, permission, CancellationToken).ConfigureAwait(false);

        if (exist)
        {
            return Result.Failure("The permission is already exists into this user");
        }

        return await userPermissionStore.AddUserToPermissionAsync(userId, permission, CancellationToken).ConfigureAwait(false);
    }
    
    public virtual async Task<Result> RemoveUserPermissionAsync(string userId, TResourcePermission permission)
    {
        ThrowIfDisposed();
        var userPermissionStore = GetUserPermissionStore();
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(permission);
        ArgumentNullException.ThrowIfNull(userPermissionStore);
        
        var exist = await userPermissionStore.IsUserInPermissionAsync(userId, permission, CancellationToken).ConfigureAwait(false);

        if (exist)
        {
            return await userPermissionStore.RemoveFromPermissionAsync(userId, permission, CancellationToken).ConfigureAwait(false);
        }
            
        return Result.Success();
    }

    public virtual async Task<bool> IsPermissionInUserAsync(string userId, TResourcePermission permission)
    {
        ThrowIfDisposed();
        var userPermissionStore = GetUserPermissionStore();
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(permission);
        ArgumentNullException.ThrowIfNull(userPermissionStore);

        return await userPermissionStore.IsUserInPermissionAsync(userId, permission, CancellationToken)
            .ConfigureAwait(false);
    }

    public virtual Task<IList<string>> GetUsersInPermissionAsync(TResourcePermission userPermission)
    {
        ThrowIfDisposed();
        var userPermissionStore = GetUserPermissionStore();
        ArgumentNullException.ThrowIfNull(userPermission);
        ArgumentNullException.ThrowIfNull(userPermissionStore);

        return userPermissionStore.GetUsersInPermissionAsync(userPermission, CancellationToken);
    }

    public virtual Task<IList<TResourcePermission>> GePermissionsInUserAsync(string userId)
    {
        ThrowIfDisposed();
        var userPermissionStore = GetUserPermissionStore();
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(userPermissionStore);
        
        return userPermissionStore.GetPermissionsInUserAsync(userId, CancellationToken);
    }

    // Role Permissions
    public virtual async Task<Result> CreateRolePermissionAsync(string roleId, TResourcePermission permission)
        => await CreateRolePermissionCoreAsync(roleId, permission).ConfigureAwait(false);

    private async Task<Result> CreateRolePermissionCoreAsync(string roleId, TResourcePermission permission)
    {
        ThrowIfDisposed();
        var rolePermissionStore = GetRolePermissionStore();
        ArgumentNullException.ThrowIfNull(roleId);
        ArgumentNullException.ThrowIfNull(rolePermissionStore);
        ArgumentNullException.ThrowIfNull(permission);
        var validateUserResult = await ValidateRolePermissionAsync(permission).ConfigureAwait(false);
        if (validateUserResult.IsFailure)
        {
            return validateUserResult;
        }
        
        var exist = await rolePermissionStore.IsRoleInPermissionAsync(roleId, permission, CancellationToken).ConfigureAwait(false);

        if (exist)
        {
            return Result.Failure("The permission is already exists into this role");
        }

        return await rolePermissionStore.AddRoleToPermissionAsync(roleId, permission, CancellationToken).ConfigureAwait(false);
    }
    
    public virtual async Task<Result> RemoveRolePermissionAsync(string roleId, TResourcePermission permission)
    {
        ThrowIfDisposed();
        var rolePermissionStore = GetRolePermissionStore();
        ArgumentNullException.ThrowIfNull(roleId);
        ArgumentNullException.ThrowIfNull(permission);
        ArgumentNullException.ThrowIfNull(rolePermissionStore);
        
        var exist = await rolePermissionStore.IsRoleInPermissionAsync(roleId, permission, CancellationToken).ConfigureAwait(false);

        if (exist)
        {
            return await rolePermissionStore.RemovePermissionFromRoleAsync(roleId, permission, CancellationToken).ConfigureAwait(false);
        }
            
        return Result.Success();
    }

    public virtual async Task<bool> IsPermissionInRoleAsync(string roleId, TResourcePermission permission)
    {
        ThrowIfDisposed();
        var rolePermissionStore = GetRolePermissionStore();
        ArgumentNullException.ThrowIfNull(roleId);
        ArgumentNullException.ThrowIfNull(permission);
        ArgumentNullException.ThrowIfNull(rolePermissionStore);

        return await rolePermissionStore.IsRoleInPermissionAsync(roleId, permission, CancellationToken)
            .ConfigureAwait(false);
    }

    public virtual Task<IList<string>> GetRolesInPermissionAsync(TResourcePermission userPermission)
    {
        ThrowIfDisposed();
        var rolePermissionStore = GetRolePermissionStore();
        ArgumentNullException.ThrowIfNull(userPermission);
        ArgumentNullException.ThrowIfNull(rolePermissionStore);

        return rolePermissionStore.GetRolesInPermissionAsync(userPermission, CancellationToken);
    }

    public virtual Task<IList<TResourcePermission>> GePermissionsInRoleAsync(string roleId)
    {
        ThrowIfDisposed();
        var rolePermissionStore = GetRolePermissionStore();
        ArgumentNullException.ThrowIfNull(roleId);
        ArgumentNullException.ThrowIfNull(rolePermissionStore);
        
        return rolePermissionStore.GetPermissionsInRoleAsync(roleId, CancellationToken);
    }

    protected async Task<Result> ValidateUserPermissionAsync(TResourcePermission permission)
    {
        List<string>? errors = null;
        foreach (var v in UserPermissionValidators)
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
                Logger.LogDebug("User Permission validation failed: {errors}.", string.Join(";", errors));
            }
            return Result.Failure(errors);
        }
        return Result.Success();
    }
    
    protected async Task<Result> ValidateRolePermissionAsync(TResourcePermission permission)
    {
        List<string>? errors = null;
        foreach (var v in RolePermissionValidators)
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
                Logger.LogDebug("Role Permission validation failed: {errors}.", string.Join(";", errors));
            }
            return Result.Failure(errors);
        }
        return Result.Success();
    }
    
    private IUserPermissionStore<TUserPermission, TResourcePermission> GetUserPermissionStore()
    {
        var cast = Store as IUserPermissionStore<TUserPermission, TResourcePermission>;
        if (cast == null)
        {
            throw new NotSupportedException("Store does not implement IUserPermissionStore");
        }
        return cast;
    }
    
    private IRolePermissionStore<TUserPermission, TResourcePermission> GetRolePermissionStore()
    {
        var cast = Store as IRolePermissionStore<TUserPermission, TResourcePermission>;
        if (cast == null)
        {
            throw new NotSupportedException("Store does not implement IRolePermissionStore");
        }
        return cast;
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
