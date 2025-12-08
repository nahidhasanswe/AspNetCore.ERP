namespace ERP.Core.Policy.Permission.Store;

public class UserPermission<TKey, TUserKey>
    where TKey : IEquatable<TKey>
    where TUserKey : IEquatable<TUserKey>
{
    public virtual TUserKey UserId { get; set; } = default!;
    public virtual TKey ResourcePermissionId { get; set; } = default!;
}
