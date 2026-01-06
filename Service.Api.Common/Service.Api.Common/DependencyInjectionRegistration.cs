using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;

namespace Service.Api.Common;

internal static class DependencyInjectionRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHostedService<Main>();
        
        services.AddSingleton<INatsConnectionService, NatsConnectionService>();

        services.AddMemoryCache();
        services.AddSingleton<ILocalCacheService, LocalCacheService>();
        services.AddSingleton<IDistributedCacheService, DistributedCacheService>();

        services.AddTransient<IMessageHandler, MessageHandler>();
        services.Decorate<IMessageHandler, MessageHandlerErrorLogging>();

        services.AddSingleton<IRestHandler, RestHandler>();
        services.AddSingleton<IQueryParameterParser, QueryParameterParser>();

        return services;
    }

    public static IOpenTelemetryBuilder AddTelemetryServices(this IServiceCollection services)
    {
        services.AddLogging(configure => configure.AddJsonConsole());
        var telemetry = services.AddOpenTelemetry().UseOtlpExporter()
            .WithTracing(tracing => tracing.AddSource(nameof(Main)));

        return telemetry;
    }
}
