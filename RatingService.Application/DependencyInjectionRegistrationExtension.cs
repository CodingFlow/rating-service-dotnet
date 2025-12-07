using Microsoft.Extensions.DependencyInjection;
using RatingService.Application.Handlers;

namespace RatingService.Application;

public static class DependencyInjectionRegistrationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IGetUsersHandler, GetUsersHandler>();
        services.AddTransient<IPostUsersHandler, PostUsersHandler>();

        return services;
    }
}
