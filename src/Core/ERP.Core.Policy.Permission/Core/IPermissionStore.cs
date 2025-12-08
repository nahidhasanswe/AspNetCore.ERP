namespace ERP.Core.Policy.Permission;

public interface IPermissionStore<TPermission> : IDisposable
    where TPermission : class
{
    Task<Result> CreateAsync(TPermission permission, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(TPermission permission, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(TPermission permission, CancellationToken cancellationToken = default);
    Task<string> GetPermissionIdAsync(TPermission resource, CancellationToken cancellationToken = default);
    Task<string?> GetActionNameAsync(TPermission resource, CancellationToken cancellationToken = default);
    Task<string?> GetScopeNameAsync(TPermission resource, CancellationToken cancellationToken = default);
    Task SetPermissionActionAsync(TPermission resource, string action, CancellationToken cancellationToken = default);
    Task SetPermissionScopeAsync(TPermission resource, string? scope, CancellationToken cancellationToken = default);
    Task<TPermission?> FindByActionAndScopeAsync(string action, string? scope, CancellationToken cancellationToken = default);
    Task<TPermission?> FindPermissionByIdAsync(string id, CancellationToken cancellationToken = default);
}
