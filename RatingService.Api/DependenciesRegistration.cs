using Microsoft.Extensions.DependencyInjection;
using RatingService.Application;

namespace RatingService.Api;

internal static class DependenciesRegistration
{
    public static void RegisterDependencies(IServiceCollection services)
    {
        services.AddApiServices();
        services.AddApplicationServices();
    }
}
