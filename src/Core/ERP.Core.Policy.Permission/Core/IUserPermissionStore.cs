namespace ERP.Core.Policy.Permission;

public interface IUserPermissionStore<TUserPermission, TResourcePermission> : IDisposable
    where TUserPermission : class
    where TResourcePermission : class
{
    Task<Result> AddUserToPermissionAsync(string userId, TResourcePermission userPermission, CancellationToken cancellationToken = default);
    Task<Result> RemoveFromPermissionAsync(string userId, TResourcePermission userPermission, CancellationToken cancellationToken = default);
    Task<bool> IsUserInPermissionAsync(string userId, TResourcePermission userPermission, CancellationToken cancellationToken = default);
    Task<IList<string>> GetUsersInPermissionAsync(TResourcePermission userPermission, CancellationToken cancellationToken = default);
    Task<IList<TResourcePermission>> GetPermissionsInUserAsync(string userId, CancellationToken cancellationToken = default);
}

public interface IQueryableUserPermissionStore<TUserPermission, TResourcePermission> : IUserPermissionStore<TUserPermission,TResourcePermission>
    where TResourcePermission : class
    where TUserPermission : class
{
    IQueryable<TUserPermission> UserPermissions { get; }
}