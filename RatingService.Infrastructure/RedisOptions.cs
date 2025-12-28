using System.ComponentModel.DataAnnotations;
using CodingFlow.OptionsBindingsGenerator;
using Microsoft.Extensions.Configuration;

namespace RatingService.Infrastructure;

[OptionsBindings(true)]
internal record RedisOptions
{
    [Required]
    [ConfigurationKeyName("REDIS_SERVICE_HOST")]
    public required string Host { get; set; }

    [Required]
    [ConfigurationKeyName("REDIS_SERVICE_PORT")]
    public required string Port { get; set; }
}