using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;

namespace Service.Api.Common;

internal static class DependencyInjectionRegistrationExtension
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHostedService<Main>();
        services.AddTransient<IMessageHandler, MessageHandler>();
        services.AddSingleton<IRestHandler, RestHandler>();
        services.AddMemoryCache();

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
