using Microsoft.Extensions.DependencyInjection;

namespace Service.Api.Common;

internal static class DependenciesRegistration
{
    public static void RegisterDependencies(IServiceCollection services)
    {
        services.AddApiServices();
    }

    internal static void RegisterAsyncApiBindings<TRequestDispatcher>(IServiceCollection services)
        where TRequestDispatcher : class, IRequestDispatcher
    {
        services.AddTransient<IRequestDispatcher, TRequestDispatcher>();
    }
}
