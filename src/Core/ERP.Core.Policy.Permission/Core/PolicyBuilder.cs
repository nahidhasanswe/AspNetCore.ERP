using Microsoft.Extensions.DependencyInjection;

namespace ERP.Core.Policy.Permission;

public class PolicyBuilder
{
    
    public Type ResourceType { get; }

    public Type? ResourcePermissionType { get; private set; }
    
    public Type? PermissionType { get; private set; }
    
    public Type? UserPermissionType { get; private set; }
    
    public Type? RolePermissionType { get; private set; }
    
    public IServiceCollection Services { get; }
    
    public PolicyBuilder(Type resourceType, Type permissionType, Type resourcePermissionType, Type userPermissionType, Type rolePermissionType, IServiceCollection services)
    {
        ResourceType = resourceType;
        PermissionType = permissionType;
        ResourcePermissionType = resourcePermissionType;
        UserPermissionType = userPermissionType;
        RolePermissionType = rolePermissionType;
        Services = services;
    }
    
    private PolicyBuilder AddScoped(Type serviceType, Type concreteType)
    {
        Services.AddScoped(serviceType, concreteType);
        return this;
    }
    
    public virtual PolicyBuilder AddResourceValidator<TValidator>() where TValidator : class
        => AddScoped(typeof(IResourceValidator<>).MakeGenericType(ResourceType), typeof(TValidator));
    
    public virtual PolicyBuilder AddPermissionValidator<TValidator>() where TValidator : class
        => AddScoped(typeof(IPermissionValidator<>).MakeGenericType(PermissionType), typeof(TValidator));

    public virtual PolicyBuilder AddUserPermissionValidator<TValidator>() where TValidator : class
        => AddScoped(typeof(IUserPermissionValidator<,,>).MakeGenericType(UserPermissionType, RolePermissionType, ResourcePermissionType), typeof(TValidator));
    
    public virtual PolicyBuilder AddRolePermissionValidator<TValidator>() where TValidator : class
        => AddScoped(typeof(IRolePermissionValidator<,,>).MakeGenericType(UserPermissionType, RolePermissionType, ResourcePermissionType), typeof(TValidator));
    
    public virtual PolicyBuilder AddResourceStore<TStore>() where TStore : class
        => AddScoped(typeof(IResourceStore<>).MakeGenericType(ResourceType), typeof(TStore));
    
    public virtual PolicyBuilder AddResourcePermissionStore<TStore>() where TStore : class
        => AddScoped(typeof(IResourcePermissionStore<>).MakeGenericType(PermissionType), typeof(TStore));
    
    public virtual PolicyBuilder AddResourceSecurityStore<TStore>() where TStore : class
        => AddScoped(typeof(IResourceSecurityStampStore<>).MakeGenericType(PermissionType), typeof(TStore));
    
    public virtual PolicyBuilder AddUserPermissionStore<TStore>() where TStore : class
        => AddScoped(typeof(IUserPermissionStore<,>).MakeGenericType(UserPermissionType, ResourcePermissionType), typeof(TStore));
    
    public virtual PolicyBuilder AddRolePermissionStore<TStore>() where TStore : class
        => AddScoped(typeof(IRolePermissionStore<,>).MakeGenericType(RolePermissionType, ResourcePermissionType), typeof(TStore));
    
    public virtual PolicyBuilder AddUserRolePermissionStore<TStore>() where TStore : class
        => AddScoped(typeof(IUserRolePermissionStore<,,>).MakeGenericType(ResourcePermissionType, UserPermissionType, RolePermissionType), typeof(TStore));
    
    public virtual PolicyBuilder AddResourceManager<TUserManager>() where TUserManager : class
    {
        var userManagerType = typeof(ResourceManager<>).MakeGenericType(ResourceType);
        var customType = typeof(TUserManager);
        if (!userManagerType.IsAssignableFrom(customType))
        {
            throw new InvalidOperationException();
        }
        if (userManagerType != customType)
        {
            Services.AddScoped(customType, services => services.GetRequiredService(userManagerType));
        }
        return AddScoped(userManagerType, customType);
    }
    
    public virtual PolicyBuilder AddUserRolePermissionManager<TUserRolePermissionManager>() where TUserRolePermissionManager : class
    {
        var userRolePermissionManagerType = typeof(UserRolePermissionManager<,,>).MakeGenericType(UserPermissionType, RolePermissionType, ResourcePermissionType);
        var customType = typeof(TUserRolePermissionManager);
        if (!userRolePermissionManagerType.IsAssignableFrom(customType))
        {
            throw new InvalidOperationException();
        }
        if (userRolePermissionManagerType != customType)
        {
            Services.AddScoped(customType, services => services.GetRequiredService(userRolePermissionManagerType));
        }
        return AddScoped(userRolePermissionManagerType, customType);
    }
}