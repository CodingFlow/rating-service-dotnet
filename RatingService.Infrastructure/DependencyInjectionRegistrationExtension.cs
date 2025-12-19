using CodingFlow.Generated.OptionsBindingsGenerator.GeneratedRatingService.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RatingService.Infrastructure;

public static class DependencyInjectionRegistrationExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddOptionsBindings(configuration);
        services.AddDbContext<RatingContext>();

        return services;
    }
}
