using Microsoft.Extensions.DependencyInjection;
using RatingService.Application.Handlers;

namespace RatingService.Application;

public static class DependencyInjectionRegistrationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IGetRatingsHandler, GetRatingsHandler>();
        services.AddTransient<IPostRatingsHandler, PostRatingsHandler>();
        services.AddTransient<IDeleteRatingsHandler, DeleteRatingsHandler>();

        return services;
    }
}
