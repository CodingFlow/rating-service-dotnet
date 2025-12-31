using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry;
using RatingService.Application;
using RatingService.Infrastructure;

namespace RatingService.AppHost;

internal static class DependenciesRegistration
{
    public static void RegisterDependencies(IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddInfrastructureServices(configuration);
        services.AddApplicationServices();
    }

    public static void RegisterTelemetry(IOpenTelemetryBuilder telemetry)
    {
        telemetry.WithTracing(tracing => tracing.AddNpgsql());
    }
}
