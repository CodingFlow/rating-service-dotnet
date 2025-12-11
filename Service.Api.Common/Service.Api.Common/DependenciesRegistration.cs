using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Api.Common;

internal static class DependenciesRegistration
{
    public static void RegisterDependencies(IServiceCollection services)
    {
        services.AddApiServices();
    }

    public static void RegisterAsyncApiBindings<TRequestDispatcher>(IServiceCollection services)
        where TRequestDispatcher : class, IRequestDispatcher
    {
        services.AddTransient<IRequestDispatcher, TRequestDispatcher>();
    }

    public static void RegisterConfiguration(IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<NatsService>(configuration);
    }
}
