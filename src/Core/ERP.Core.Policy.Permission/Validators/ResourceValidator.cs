namespace ERP.Core.Policy.Permission.Validators;

public class ResourceValidator<TResource> : IResourceValidator<TResource>
    where TResource : class
{
    public async Task<Result> ValidateAsync(ResourceManager<TResource> manager, TResource resource, CancellationToken cancellationToken = default)
    {
        List<string>? errors = null;
        
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(resource);

        var result = await ValidateResourceNameAsync(manager, resource);

        if (result.IsFailure)
        {
            errors ??= new List<string>();
            errors.Add(result.Error);
        }
        
        return errors?.Count > 0 ? Result.Failure(errors) : Result.Success();
    }

    private async Task<Result> ValidateResourceNameAsync(ResourceManager<TResource> manager, TResource resource)
    {
        var name = await manager.GetResourceNameAsync(resource).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure("Resource name is null or empty");
        }

        var existing = await manager.FindByNameAsync(name);

        if (existing is not null)
        {
            return Result.Failure($"Resource name {name} is already exists");
        }
        
        return Result.Success();
    }
}