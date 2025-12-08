using ERP.Application.Configurations;
using ERP.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ERP.Api.Configuration;

public static class DependencyConfig
{
    public static IServiceCollection AddIoC(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Repository and DB Context
        services.AddRepositories(x =>
        {
            x.UseInMemoryDatabase("ERPSystem");
        });
        
        // Fluent Validation
        services.AddFluentValidationConfig();
        
        // MediatR Config
        services.AddMediatRConfiguration();

        return services;
    }
}