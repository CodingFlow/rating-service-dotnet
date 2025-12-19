using CodingFlow.Generated.OptionsBindingsGenerator.GeneratedService.Api.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.AppHost.Common;

namespace Service.Api.Common;

public class CommonDependenciesRegistration<TRequestDispatcher> : ICommonDependenciesRegistration
    where TRequestDispatcher : class, IRequestDispatcher
{
    public void RegisterConfiguration(IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddOptionsBindings(configuration);
        services.AddTransient<IRequestDispatcher, TRequestDispatcher>();
    }

    public void RegisterDependencies(IServiceCollection services)
    {
        services.AddApiServices();
    }
}
