using Microsoft.Extensions.DependencyInjection;

namespace RatingService.Api;

internal static class DependencyInjectionRegistrationExtension
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddTransient<IMain, Main>();
        services.AddTransient<IMainHandler, MainHandler>();

        return services;
    }
}
