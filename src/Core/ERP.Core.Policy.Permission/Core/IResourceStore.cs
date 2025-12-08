namespace ERP.Core.Policy.Permission;

public interface IResourceStore<TResource> : IDisposable
    where TResource : class
{
    Task<string> GetResourceIdAsync(TResource resource, CancellationToken cancellationToken = default);
    Task<string?> GetResourceNameAsync(TResource resource, CancellationToken cancellationToken = default);
    Task SetResourceNameAsync(TResource resource, string? name, CancellationToken cancellationToken = default);
    Task<Result> CreateAsync(TResource resource, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(TResource resource, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(TResource resource, CancellationToken cancellationToken = default);
    Task<TResource?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<TResource?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
}