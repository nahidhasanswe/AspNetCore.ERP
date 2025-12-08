namespace ERP.Core.Policy.Permission.Store;


public class RolePermission<TKey, TRoleKey>
    where TKey : IEquatable<TKey>
    where TRoleKey : IEquatable<TRoleKey>
{
    public virtual TRoleKey RoleId { get; set; } = default!;
    public virtual TKey ResourcePermissionId { get; set; } = default!;
}
