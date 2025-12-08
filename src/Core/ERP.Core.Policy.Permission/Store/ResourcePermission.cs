namespace ERP.Core.Policy.Permission.Store;


public class ResourcePermission : ResourcePermission<string>
{
    public ResourcePermission()
    {
        ResourcePermissionId = Guid.NewGuid().ToString();
    }

    public ResourcePermission(string resourceId, string permissionId) : this()
    {
        ResourceId = resourceId;
        PermissionId = permissionId;
    }
}

public class ResourcePermission<TKey>
    where TKey : IEquatable<TKey>
{
    public ResourcePermission()
    {
        
    }
    
    public virtual TKey ResourcePermissionId { get; set; } = default!;
    
    public virtual TKey ResourceId { get; set; } = default!;

    public virtual TKey PermissionId { get; set; } = default!;
}
