using Microsoft.Extensions.DependencyInjection;
using Service.AppHost.Common;

namespace Service.Api.Common;

internal static class DependencyInjectionRegistrationExtension
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddTransient<IMain, Main>();
        services.AddTransient<IMainHandler, MainHandler>();

        return services;
    }
}
