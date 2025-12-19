using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Service.AppHost.Common;

public static class Starter
{
    public static async Task Start(string[] args, Action<IServiceCollection, IConfigurationManager> registerDependencies, ICommonDependenciesRegistration commonDependenciesRegistration)
    {
        Console.WriteLine($"Beginning program");

        var builder = Host.CreateApplicationBuilder(args);

        commonDependenciesRegistration.RegisterDependencies(builder.Services);
        commonDependenciesRegistration.RegisterConfiguration(builder.Services, builder.Configuration);
        registerDependencies(builder.Services, builder.Configuration);

        using IHost host = builder.Build();

        using var serviceScope = host.Services.CreateScope();
        var provider = serviceScope.ServiceProvider;

        var main = provider.GetRequiredService<IMain>();

        await main.Run();

        await host.RunAsync();
    }
}
