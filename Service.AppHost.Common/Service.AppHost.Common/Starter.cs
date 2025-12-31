using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;

namespace Service.AppHost.Common;

public static class Starter
{
    public static async Task Start(
        string[] args,
        Action<IServiceCollection, IConfigurationManager> registerDependencies,
        Action<IOpenTelemetryBuilder> registerTelemetry,
        ICommonDependenciesRegistration commonDependenciesRegistration)
    {
        Console.WriteLine($"Beginning program");

        var builder = Host.CreateApplicationBuilder(args);

        commonDependenciesRegistration.RegisterDependencies(builder.Services);
        commonDependenciesRegistration.RegisterConfiguration(builder.Services, builder.Configuration);
        var telemetry = commonDependenciesRegistration.RegisterTelemetry(builder.Services);
        
        registerDependencies(builder.Services, builder.Configuration);
        registerTelemetry(telemetry);

        using var host = builder.Build();

        await host.RunAsync();
    }
}
