namespace ERP.Core.Policy.Permission;

public interface IResourceSecurityStampStore<TResource> : IResourceStore<TResource>
    where TResource : class
{
    Task SetSecurityStampAsync(TResource resource, string stamp, CancellationToken cancellationToken);
    
    Task<string?> GetSecurityStampAsync(TResource resource, CancellationToken cancellationToken);
}