using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RatingService.Domain;

namespace RatingService.Infrastructure;

internal class RatingContext(IOptions<PostgreSqlDatabaseOptions> databaseOptions) : DbContext
{
    private readonly PostgreSqlDatabaseOptions databaseSettings = databaseOptions.Value;

    public DbSet<Rating> Ratings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = $"Host={databaseSettings.HostReadWrite};Username={databaseSettings.Username};Password={databaseSettings.Password};Database={databaseSettings.DatabaseName}";
        
        optionsBuilder.UseNpgsql(connectionString);
    }
}
