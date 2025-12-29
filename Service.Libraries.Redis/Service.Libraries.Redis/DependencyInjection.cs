using CodingFlow.Generated.OptionsBindingsGenerator.GeneratedService.Libraries.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Abstractions;

namespace Service.Libraries.Redis;

public static class DependencyInjection
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddOptionsBindings(configuration);

        services.AddSharedSingleton<IStartupService, IRedisConnection, RedisConnection>();
        services.AddSingleton<IRedisContext, RedisContext>();

        return services;
    }
}
