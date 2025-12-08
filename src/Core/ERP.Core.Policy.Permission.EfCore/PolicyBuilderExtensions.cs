using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ERP.Core.Policy.Permission.EfCore;

public static class PolicyBuilderExtensions
{
    public static PolicyBuilder AddEntityFrameworkStores<TContext>(this PolicyBuilder builder)
        where TContext : DbContext
    {
        AddStores(builder.Services, builder.ResourceType, builder.PermissionType, builder.ResourcePermissionType, builder.UserPermissionType, builder.RolePermissionType, typeof(TContext));
        return builder;
    }

    private static void AddStores(IServiceCollection services, Type resourceType, Type permissionType,  Type resourcePermissionType, Type userPermissionType,  Type rolePermissionType, Type contextType)
    {
        var genericResourceType = FindGenericBaseType(resourceType, typeof(Store.Resource<>));
        if (genericResourceType == null)
        {
            throw new InvalidOperationException();
        }

        var keyType = genericResourceType.GenericTypeArguments[0];

        var genericPermissionType = FindGenericBaseType(permissionType, typeof(Store.Permission<>));
        if (genericPermissionType == null)
        {
            throw new InvalidOperationException();
        }

        var genericUserPermissionType = FindGenericBaseType(userPermissionType, typeof(Store.UserPermission<,>));
        if (genericUserPermissionType == null)
        {
            throw new InvalidOperationException();
        }
        
        var genericRolePermissionType = FindGenericBaseType(rolePermissionType, typeof(Store.RolePermission<,>));
        if (genericRolePermissionType == null)
        {
            throw new InvalidOperationException();
        }

        Type resourceStoreType;
        Type userRolePermissionStoreType;
        
        var policyDbContext = FindGenericBaseType(contextType, typeof(PolicyPermissionDbContext<,,,,,,,>));
        if (policyDbContext == null)
        {
            // If its a custom DbContext, we can only add the default POCOs
            resourceStoreType = typeof(ResourceStore<,,,,>).MakeGenericType(keyType, resourceType, permissionType, resourcePermissionType, contextType);
            userRolePermissionStoreType = typeof(UserRolePermissionStore<,,,,,,>).MakeGenericType(keyType, genericUserPermissionType.GenericTypeArguments[1], genericRolePermissionType.GenericTypeArguments[1], userPermissionType, rolePermissionType, resourcePermissionType, contextType);
        }
        else
        {
            resourceStoreType = typeof(ResourceStore<,,,,>).MakeGenericType(
                policyDbContext.GenericTypeArguments[0], 
                policyDbContext.GenericTypeArguments[3], 
                policyDbContext.GenericTypeArguments[4], 
                policyDbContext.GenericTypeArguments[5], contextType);
            
            userRolePermissionStoreType = typeof(UserRolePermissionStore<,,,,,,>).MakeGenericType(
                policyDbContext.GenericTypeArguments[0],
                policyDbContext.GenericTypeArguments[1],
                policyDbContext.GenericTypeArguments[2],
                policyDbContext.GenericTypeArguments[6],
                policyDbContext.GenericTypeArguments[7],
                policyDbContext.GenericTypeArguments[5],
                contextType);
        }
        services.TryAddScoped(typeof(IResourceStore<>).MakeGenericType(resourceType), resourceStoreType);
        services.TryAddScoped(typeof(IUserRolePermissionStore<,,>).MakeGenericType(resourcePermissionType, userPermissionType, rolePermissionType), userRolePermissionStoreType);
    }

    private static Type? FindGenericBaseType(Type currentType, Type genericBaseType)
    {
        Type? type = currentType;
        while (type != null)
        {
            var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            if (genericType != null && genericType == genericBaseType)
            {
                return type;
            }
            type = type.BaseType;
        }
        return null;
    }
}