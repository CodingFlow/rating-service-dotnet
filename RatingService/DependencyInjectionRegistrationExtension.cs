using Microsoft.Extensions.DependencyInjection;
using RatingService.Handlers;

namespace RatingService;

internal static class DependencyInjectionRegistrationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IMain, Main>();
        services.AddTransient<IGetUsersHandler, GetUsersHandler>();
        services.AddTransient<IPostUsersHandler, PostUsersHandler>();

        return services;
    }
}
