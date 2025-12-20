using System.ComponentModel.DataAnnotations;
using CodingFlow.OptionsBindingsGenerator;
using Microsoft.Extensions.Configuration;

namespace RatingService.Infrastructure;

[OptionsBindings(true)]
internal record PostgreSqlDatabaseOptions
{
    [Required]
    [ConfigurationKeyName("DATABASE_POSTGRESQL_HOST")]
    public required string Host { get; set; }

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