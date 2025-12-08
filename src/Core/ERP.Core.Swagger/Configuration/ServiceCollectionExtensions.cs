using ERP.Core.Swagger.Filters;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ERP.Core.Swagger.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, Action<SwaggerGenOptions> options, Type? genericResponseType = null)
    {
        services.AddSingleton<SwaggerApiGenericResponseConfig>(options => new SwaggerApiGenericResponseConfig(genericResponseType ?? typeof(ServiceCollectionExtensions) ));
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            // 1. Add the internal filters you requested
            c.OperationFilter<CancellationTokenFilter>();
            c.OperationFilter<SwaggerTypeSetterFilter>();
            c.OperationFilter<SwaggerResponseTypesSetterFilter>();

            // 2. Apply the external options provided by the user (e.g. Title, Version, Auth)
            options?.Invoke(c);
        });


        return services;
    }
}