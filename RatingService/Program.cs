using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RatingService;

Console.WriteLine($"Beginning program");

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplicationServices();

using IHost host = builder.Build();

using var serviceScope = host.Services.CreateScope();
var provider = serviceScope.ServiceProvider;

var main = provider.GetRequiredService<IMain>();

_ = main.Run();

await host.RunAsync();
