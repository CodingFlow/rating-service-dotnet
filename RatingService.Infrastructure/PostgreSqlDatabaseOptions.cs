using System.ComponentModel.DataAnnotations;
using CodingFlow.OptionsBindingsGenerator;
using Microsoft.Extensions.Configuration;

namespace RatingService.Infrastructure;

[OptionsBindings(true)]
internal record PostgreSqlDatabaseOptions
{
    [Required]
    [ConfigurationKeyName("DATABASE_POSTGRESQL_URI")]
    public required string DatabaseConnectionString { get; set; }
}