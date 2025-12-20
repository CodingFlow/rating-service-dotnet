using Microsoft.Extensions.DependencyInjection;

namespace Service.Api.Common;

internal static class DependencyInjectionRegistrationExtension
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHostedService<Main>();
        services.AddTransient<IMainHandler, MainHandler>();

        return services;
    }
}
