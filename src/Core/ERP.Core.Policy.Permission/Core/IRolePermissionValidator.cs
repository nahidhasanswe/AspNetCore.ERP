namespace ERP.Core.Policy.Permission;

public interface IRolePermissionValidator<TUserPermission, TRolePermission, TResourcePermission>
    where TUserPermission : class
    where TRolePermission : class
    where TResourcePermission : class
{
    Task<Result> ValidateAsync(UserRolePermissionManager<TUserPermission, TRolePermission, TResourcePermission> manager, TResourcePermission rolePermission, CancellationToken cancellationToken = default);
}


