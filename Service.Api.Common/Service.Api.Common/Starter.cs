using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Service.Api.Common;

public static class Starter
{
    public static async Task Start<TAsyncApiBindingsClass>(string[] args, Action<IServiceCollection> registerDependencies)
        where TAsyncApiBindingsClass: class, IRequestDispatcher
    {
        Console.WriteLine($"Beginning program");

        var builder = Host.CreateApplicationBuilder(args);

        DependenciesRegistration.RegisterDependencies(builder.Services);
        DependenciesRegistration.RegisterAsyncApiBindings<TAsyncApiBindingsClass>(builder.Services);
        DependenciesRegistration.RegisterConfiguration(builder.Services, builder.Configuration);
        registerDependencies(builder.Services);

        using IHost host = builder.Build();

        using var serviceScope = host.Services.CreateScope();
        var provider = serviceScope.ServiceProvider;

        var main = provider.GetRequiredService<IMain>();

        _ = main.Run();

        await host.RunAsync();
    }
}
