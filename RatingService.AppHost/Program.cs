using RatingService.Api;
using Service.Api.Common;
using Service.AppHost.Common;

Console.WriteLine("Starting Rating Service API...");

await Starter.Start(
    args,
    RatingService.AppHost.DependenciesRegistration.RegisterDependencies,
    RatingService.AppHost.DependenciesRegistration.RegisterTelemetry,
    new CommonDependenciesRegistration<RequestDispatcher>());
