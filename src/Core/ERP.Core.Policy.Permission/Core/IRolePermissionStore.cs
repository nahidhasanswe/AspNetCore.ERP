namespace ERP.Core.Policy.Permission;
public interface IRolePermissionStore<TRolePermission, TResourcePermission> : IDisposable
    where TResourcePermission : class
    where TRolePermission : class
{
    Task<Result> AddRoleToPermissionAsync(string roleId, TResourcePermission rolePermission, CancellationToken cancellationToken = default);
    Task<Result> RemovePermissionFromRoleAsync(string roleId, TResourcePermission rolePermission, CancellationToken cancellationToken = default);
    Task<bool> IsRoleInPermissionAsync(string roleId, TResourcePermission rolePermission, CancellationToken cancellationToken = default);
    Task<IList<string>> GetRolesInPermissionAsync(TResourcePermission rolePermission, CancellationToken cancellationToken = default);
    Task<IList<TResourcePermission>> GetPermissionsInRoleAsync(string roleId, CancellationToken cancellationToken = default);
}

public interface IQueryableRolePermissionStore<TRolePermission, TResourcePermission> : IRolePermissionStore<TRolePermission, TResourcePermission>
    where TResourcePermission : class
    where TRolePermission : class
{
    IQueryable<TRolePermission> RolePermissions { get; }
}