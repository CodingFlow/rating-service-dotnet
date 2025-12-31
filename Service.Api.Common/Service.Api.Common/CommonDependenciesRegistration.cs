using CodingFlow.Generated.OptionsBindingsGenerator.GeneratedService.Api.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using Service.AppHost.Common;
using Service.Libraries.Redis;

namespace Service.Api.Common;

public class CommonDependenciesRegistration<TRequestDispatcher> : ICommonDependenciesRegistration
    where TRequestDispatcher : class, IRequestDispatcher
{
    public void RegisterConfiguration(IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddOptionsBindings(configuration);
        services.AddRedis(configuration);
    }

    public void RegisterDependencies(IServiceCollection services)
    {
        services.AddTransient<IRequestDispatcher, TRequestDispatcher>();
        services.AddApiServices();
    }

    public IOpenTelemetryBuilder RegisterTelemetry(IServiceCollection services)
    {
        return services.AddTelemetryServices();
    }
}
