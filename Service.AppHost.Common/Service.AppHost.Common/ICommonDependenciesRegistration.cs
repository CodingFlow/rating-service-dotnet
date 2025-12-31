using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;

namespace Service.AppHost.Common;

public interface ICommonDependenciesRegistration
{
    void RegisterConfiguration(IServiceCollection services, IConfigurationManager configuration);
    void RegisterDependencies(IServiceCollection services);

    IOpenTelemetryBuilder RegisterTelemetry(IServiceCollection services);
}