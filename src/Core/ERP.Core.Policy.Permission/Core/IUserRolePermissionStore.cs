namespace ERP.Core.Policy.Permission;

public interface IUserRolePermissionStore<TResourcePermission, TUserPermission, TRolePermission> :
    IUserPermissionStore<TUserPermission, TResourcePermission>,
    IRolePermissionStore<TRolePermission, TResourcePermission>
    where TResourcePermission : class
    where TUserPermission : class
    where TRolePermission : class
{
    
}