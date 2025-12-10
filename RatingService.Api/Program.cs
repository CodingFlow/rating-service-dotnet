using RatingService.Api;
using Service.Api.Common;

Console.WriteLine("Starting Rating Service API...");

await Starter.Start<RequestDispatcher>(args, DependenciesRegistration.RegisterDependencies);
