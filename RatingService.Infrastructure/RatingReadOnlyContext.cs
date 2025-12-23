using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RatingService.Domain;

namespace RatingService.Infrastructure;

internal class RatingReadOnlyContext(IOptions<PostgreSqlDatabaseOptions> databaseOptions) : DbContext
{
    private readonly PostgreSqlDatabaseOptions databaseSettings = databaseOptions.Value;

    public DbSet<Rating> Ratings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = $"Host={databaseSettings.HostReadOnly};Username={databaseSettings.Username};Password={databaseSettings.Password};Database={databaseSettings.DatabaseName}";
        
        Console.WriteLine($"RatingContext - environment variable connection string: {connectionString}");
        
        optionsBuilder.UseNpgsql(connectionString);
    }
}
