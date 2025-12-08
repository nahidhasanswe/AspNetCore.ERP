namespace ERP.Core.Policy.Permission;

public interface IQueryablePermissionStore<TPermission> : IPermissionStore<TPermission>
    where TPermission : class
{
    IQueryable<TPermission> Permissions { get; }
}