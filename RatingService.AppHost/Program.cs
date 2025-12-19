using RatingService.Api;
using RatingService.AppHost;
using Service.Api.Common;
using Service.AppHost.Common;

Console.WriteLine("Starting Rating Service API...");

await Starter.Start(args, DependenciesRegistration.RegisterDependencies, new CommonDependenciesRegistration<RequestDispatcher>());
