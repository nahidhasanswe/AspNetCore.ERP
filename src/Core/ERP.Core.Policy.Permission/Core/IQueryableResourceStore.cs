namespace ERP.Core.Policy.Permission;

public interface IQueryableResourceStore<TResource> : IResourceStore<TResource>
    where TResource : class
{
    IQueryable<TResource> Resources { get; }
}
