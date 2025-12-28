using CodingFlow.Generated.OptionsBindingsGenerator.GeneratedRatingService.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RatingService.Domain;
using Service.Abstractions;

namespace RatingService.Infrastructure;

public static class DependencyInjectionRegistrationExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddOptionsBindings(configuration);

        services.AddDbContext<RatingContext>();
        services.AddScoped<IRatingRepository, RatingRepository>();
        services.Decorate<IRatingRepository, CachedRatingRepository>();

        services.AddDbContext<RatingReadOnlyContext>();
        services.AddScoped<IRatingReadOnlyRepository, RatingReadOnlyRepository>();
        services.Decorate<IRatingReadOnlyRepository, CachedRatingReadOnlyRepository>();

        services.AddSharedSingleton<IStartupService, IRedisConnection, RedisConnection>();

        services.AddSingleton<IRedisGetConsistent, RedisGetConsistent>();

        return services;
    }
}
