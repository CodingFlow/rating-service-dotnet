using Microsoft.Extensions.Configuration;

namespace Service.Api.Common;

internal class NatsService
{
    [ConfigurationKeyName("NATS_SERVICE_HOST")]
    public string ServiceHost { get; set; }
    
    [ConfigurationKeyName("NATS_SERVICE_PORT")]
    public string Port { get; set; }
}
