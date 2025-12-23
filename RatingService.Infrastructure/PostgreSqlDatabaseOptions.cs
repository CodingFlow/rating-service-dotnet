using System.ComponentModel.DataAnnotations;
using CodingFlow.OptionsBindingsGenerator;
using Microsoft.Extensions.Configuration;

namespace RatingService.Infrastructure;

[OptionsBindings(true)]
internal record PostgreSqlDatabaseOptions
{
    [Required]
    [ConfigurationKeyName("CLUSTER_EXAMPLE_R_SERVICE_HOST")]
    public required string HostReadOnly { get; set; }

    [Required]
    [ConfigurationKeyName("CLUSTER_EXAMPLE_RW_SERVICE_HOST")]
    public required string HostReadWrite { get; set; }

    [Required]
    [ConfigurationKeyName("CLUSTER_EXAMPLE_RO_SERVICE_HOST")]
    public required string HostAny { get; set; }

    [Required]
    [ConfigurationKeyName("DATABASE_POSTGRESQL_USERNAME")]
    public required string Username { get; set; }

    [Required]
    [ConfigurationKeyName("DATABASE_POSTGRESQL_PASSWORD")]
    public required string Password { get; set; }

    [Required]
    [ConfigurationKeyName("DATABASE_POSTGRESQL_DATABASENAME")]
    public required string DatabaseName { get; set; }
}