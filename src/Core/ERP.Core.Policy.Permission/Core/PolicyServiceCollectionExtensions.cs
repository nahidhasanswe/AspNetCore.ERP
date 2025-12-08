using ERP.Core.Policy.Permission.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ERP.Core.Policy.Permission;

public static class PolicyServiceCollectionExtensions
{

    public static PolicyBuilder AddDefaultPolicyResource(this IServiceCollection services)
        => AddDefaultPolicyResource<string, string, string>(services);
    
    public static PolicyBuilder AddDefaultPolicyResource<TKey, TUserKey, TRoleKey> (this IServiceCollection services)
        where TKey : IEquatable<TKey>
        where TUserKey : IEquatable<TUserKey>
        where TRoleKey : IEquatable<TRoleKey>

        => AddPolicyPermission<Store.Resource<TKey>, Store.Permission<TKey>, Store.UserPermission<TKey, TUserKey>, Store.RolePermission<TKey, TRoleKey>, Store.ResourcePermission<TKey>>(services);

    public static PolicyBuilder AddPolicyPermission<TResource, TPermission, TUserPermission, TRolePermission, TResourcePermission>(this IServiceCollection services)
        where TResource : class
        where TPermission : class
        where TUserPermission : class
        where TRolePermission : class
        where TResourcePermission : class
    {
        services.TryAddScoped<IResourceValidator<TResource>, ResourceValidator<TResource>>();
        services.TryAddScoped<IPermissionValidator<TPermission>, PermissionValidator<TPermission>>();
        
        services.TryAddScoped<ResourceManager<TResource>>();
        services.TryAddScoped<PermissionManager<TPermission>>();
        services.TryAddScoped<UserRolePermissionManager<TUserPermission, TRolePermission, TResourcePermission>>();
        
        return new PolicyBuilder(typeof(TResource), typeof(TPermission), typeof(TResourcePermission), typeof(TUserPermission), typeof(TRolePermission), services);
    }
}