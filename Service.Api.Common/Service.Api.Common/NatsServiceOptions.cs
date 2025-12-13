using System.ComponentModel.DataAnnotations;
using CodingFlow.OptionsBindingsGenerator;
using Microsoft.Extensions.Configuration;

namespace Service.Api.Common;

[OptionsBindings(true)]
internal record NatsServiceOptions
{
    [Required]
    [ConfigurationKeyName("NATS_SERVICE_HOST")]
    public required string ServiceHost { get; init; }

    [Required]
    [ConfigurationKeyName("NATS_SERVICE_PORT")]
    public required string Port { get; init; }
}
