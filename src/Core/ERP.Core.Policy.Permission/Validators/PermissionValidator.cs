namespace ERP.Core.Policy.Permission.Validators;

public class PermissionValidator<TPermission> : IPermissionValidator<TPermission>
    where TPermission : class
{
    public async Task<Result> ValidateAsync(PermissionManager<TPermission> manager, TPermission permission, CancellationToken cancellationToken = default)
    {
        List<string>? errors = null;
        
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(permission);

        var result = await ValidatePermissionActionAndScopeAsync(manager, permission);

        if (result.IsFailure)
        {
            errors ??= new List<string>();
            errors.Add(result.Error);
        }
        
        return errors?.Count > 0 ? Result.Failure(errors) : Result.Success();
    }

    private async Task<Result> ValidatePermissionActionAndScopeAsync(PermissionManager<TPermission> manager, TPermission permission)
    {
        var actionName = await manager.GetActionNameAsync(permission);
        var scopeName = await manager.GetScopeNameAsync(permission);

        var existing = await manager.FindByNameAsync(actionName, scopeName);

        if (existing is not null)
        {
            return Result.Failure("Permission already exists");
        }
        
        return Result.Success();
    }
}