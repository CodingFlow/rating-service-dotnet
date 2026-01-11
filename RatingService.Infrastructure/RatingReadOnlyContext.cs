using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RatingService.Domain;

namespace RatingService.Infrastructure;

internal class RatingReadOnlyContext(IOptions<PostgreSqlDatabaseOptions> databaseOptions) : DbContext
{
    private readonly PostgreSqlDatabaseOptions databaseSettings = databaseOptions.Value;

    public DbSet<Rating> Ratings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rating>(builder =>
        {
            builder.Property<int>("InternalId")
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            builder.HasKey("InternalId");

            builder.Property(rating => rating.Id)
                .HasColumnName("RatingId")
                .HasVogenConversion();

            builder.HasAlternateKey(x => x.Id);

            builder.Property(rating => rating.UserId).HasVogenConversion();
            builder.Property(rating => rating.ServiceId).HasVogenConversion();
            builder.Property(rating => rating.Score).HasVogenConversion();
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = $"Host={databaseSettings.HostAnyRead};Username={databaseSettings.Username};Password={databaseSettings.Password};Database={databaseSettings.DatabaseName}";
        
        optionsBuilder.UseNpgsql(connectionString);
    }
}
