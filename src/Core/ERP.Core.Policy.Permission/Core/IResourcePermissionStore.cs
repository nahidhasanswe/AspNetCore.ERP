namespace ERP.Core.Policy.Permission;

public interface IResourcePermissionStore<TResource> : IResourceStore<TResource>
    where TResource: class
{
    Task AddPermissionAsync(TResource resource, string action, string? scope, CancellationToken cancellationToken = default);
    Task RemovePermissionAsync(TResource resource, string action, string? scope, CancellationToken cancellationToken = default);
    Task<IList<string>> GetPermissionsAsync(string action, string? scope, CancellationToken cancellationToken = default);
    Task<bool> IsInPermissionAsync(TResource resource, string action, string? scope, CancellationToken cancellationToken = default);
    Task<IList<TResource>> GetResourcesInPermissionAsync(string action, string? scope, CancellationToken cancellationToken = default);
}