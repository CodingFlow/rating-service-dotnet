using Microsoft.Extensions.DependencyInjection;

namespace RatingService;

internal static class DependencyInjectionRegistrationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IMain, Main>();

        return services;
    }
}
