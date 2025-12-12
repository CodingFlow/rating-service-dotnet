using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        services.AddOptionsWithValidateOnStart<NatsServiceOption>().Bind(configuration);
        services.AddOptionsWithValidateOnStart<ServiceStreamConsumerOption>().Bind(configuration);

        services.AddSingleton<IValidateOptions<NatsServiceOption>, ValidateNatsServiceOption>();
        services.AddSingleton<IValidateOptions<ServiceStreamConsumerOption>, ValidateServiceStreamConsumerOption>();
    }
}
