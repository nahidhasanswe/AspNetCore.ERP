using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ERP.Core.Policy.Permission.Store;

public abstract class UserRolePermissionStoreBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TKey,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TUserKey, 
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TRoleKey, 
    TUserPermission, 
    TRolePermission, 
    TResourcePermission> :
    IUserRolePermissionStore<TResourcePermission, TUserPermission, TRolePermission>,
    IQueryableUserPermissionStore<TUserPermission, TResourcePermission>,
    IQueryableRolePermissionStore<TRolePermission, TResourcePermission> 
    
    where TKey : IEquatable<TKey>
    where TUserKey : IEquatable<TUserKey>
    where TRoleKey : IEquatable<TRoleKey>
    where TUserPermission : UserPermission<TKey, TUserKey>, new()
    where TRolePermission : RolePermission<TKey, TRoleKey>, new()
    where TResourcePermission : ResourcePermission<TKey>, new()
{
    private bool _disposed;
    
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


    public abstract Task<Result> AddUserToPermissionAsync(string userId, TResourcePermission userPermission,
        CancellationToken cancellationToken = default);

    public abstract Task<Result> RemoveFromPermissionAsync(string userId, TResourcePermission userPermission,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> IsUserInPermissionAsync(string userId, TResourcePermission userPermission,
        CancellationToken cancellationToken = default);

    public abstract Task<IList<string>> GetUsersInPermissionAsync(TResourcePermission userPermission,
        CancellationToken cancellationToken = default);

    public abstract Task<IList<TResourcePermission>> GetPermissionsInUserAsync(string userId, CancellationToken cancellationToken = default);

    public abstract Task<Result> AddRoleToPermissionAsync(string roleId, TResourcePermission rolePermission,
        CancellationToken cancellationToken = default);

    public abstract Task<Result> RemovePermissionFromRoleAsync(string roleId, TResourcePermission rolePermission,
        CancellationToken cancellationToken = default);

    public abstract Task<bool> IsRoleInPermissionAsync(string roleId, TResourcePermission rolePermission,
        CancellationToken cancellationToken = default);

    public abstract Task<IList<string>> GetRolesInPermissionAsync(TResourcePermission rolePermission,
        CancellationToken cancellationToken = default);
    
    public abstract Task<IList<TResourcePermission>> GetPermissionsInRoleAsync(string roleId, CancellationToken cancellationToken = default);

    public abstract IQueryable<TUserPermission> UserPermissions { get; }
    public abstract IQueryable<TRolePermission> RolePermissions { get; }
    
    protected virtual string ConvertIdToString<TKeyType>(TKeyType id)
    {
        if (object.Equals(id, default(TKeyType)))
        {
            return string.Empty;
        }
        return id.ToString() ?? string.Empty;
    }
    
    protected virtual TKeyType? ConvertIdFromString<TKeyType>(string? id)
    {
        if (id == null)
        {
            return default(TKeyType);
        }
        return (TKeyType?)TypeDescriptor.GetConverter(typeof(TKeyType)).ConvertFromInvariantString(id);
    }
}