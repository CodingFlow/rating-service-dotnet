using System.Diagnostics.CodeAnalysis;
using CodingFlow.Generated.OptionsBindingsGenerator.GeneratedService.Api.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Api.Common;

internal static class DependenciesRegistration
{
    public static void RegisterDependencies(IServiceCollection services)
    {
        services.AddApiServices();
    }

    public static void RegisterAsyncApiBindings<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TRequestDispatcher>(IServiceCollection services)
        where TRequestDispatcher : class, IRequestDispatcher
    {
        services.AddTransient<IRequestDispatcher, TRequestDispatcher>();
    }

    public static void RegisterConfiguration(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddOptionsBindings(configuration);
    }
}
