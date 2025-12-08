using System.Reflection;
using ERP.Core.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Core.AutoMapper;

public static class AutoMapperServiceCollections
{
    public static void AddObjectMapper(this IServiceCollection services, string licenseKey, params Assembly[] assemblies)
    {
        var customAssemblies = assemblies.ToList();
        customAssemblies.Add(typeof(DefaultMappings).Assembly);
        services.AddAutoMapper(x =>
        {
            x.LicenseKey = licenseKey;
        }, customAssemblies);
        services.AddTransient<IObjectMapper, AutoMapperObjectMapper>();
    }
}