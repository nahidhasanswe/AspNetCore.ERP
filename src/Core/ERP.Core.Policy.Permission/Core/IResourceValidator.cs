namespace ERP.Core.Policy.Permission;

public interface IResourceValidator<TResource>
    where TResource : class
{
    Task<Result> ValidateAsync(ResourceManager<TResource> manager, TResource resource, CancellationToken cancellationToken = default);
}
