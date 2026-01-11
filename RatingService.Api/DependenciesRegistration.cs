using Microsoft.Extensions.DependencyInjection;

namespace RatingService.Api;

public static class DependenciesRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddResponseStrategies();
        services.AddRequestDecorators();
        services.AddValidators();

        return services;
    }
}
