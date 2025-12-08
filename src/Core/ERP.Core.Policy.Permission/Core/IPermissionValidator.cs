namespace ERP.Core.Policy.Permission;

public interface IPermissionValidator<TPermission> 
    where TPermission : class
{
    Task<Result> ValidateAsync(PermissionManager<TPermission> manager, TPermission permission, CancellationToken cancellationToken = default);
}