using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace Service.Api.Common;

internal record ServiceStreamConsumerOption
{
    [Required]
    [ConfigurationKeyName("SERVICE_STREAM_NAME")]
    public required string StreamName { get; init; }

    [Required]
    [ConfigurationKeyName("SERVICE_CONSUMER_NAME")]
    public required string ConsumerName { get; init; }
}
