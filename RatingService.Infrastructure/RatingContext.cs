using Microsoft.EntityFrameworkCore;
using RatingService.Domain;

namespace RatingService.Infrastructure;

public class RatingContext : DbContext
{
    public DbSet<Rating> Ratings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("");
    }
}
