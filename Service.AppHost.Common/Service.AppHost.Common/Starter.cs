using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Service.AppHost.Common;

public static class Starter
{
    public static async Task Start(string[] args, Action<IServiceCollection, IConfigurationManager> registerDependencies, ICommonDependenciesRegistration commonDependenciesRegistration)
    {
        Console.WriteLine($"Beginning program");

        var builder = Host.CreateApplicationBuilder(args);

        builder.Logging.AddJsonConsole();

        commonDependenciesRegistration.RegisterDependencies(builder.Services);
        commonDependenciesRegistration.RegisterConfiguration(builder.Services, builder.Configuration);
        registerDependencies(builder.Services, builder.Configuration);

        using var host = builder.Build();

        await host.RunAsync();
    }
}
