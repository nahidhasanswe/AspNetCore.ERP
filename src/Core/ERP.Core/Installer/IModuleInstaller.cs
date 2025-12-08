using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Core.Installer;

public interface IModuleInstaller
{
    void InstallServices(IServiceCollection services, IConfiguration configuration);
}