using Microsoft.EntityFrameworkCore;
using RatingService.Domain;

namespace RatingService.Infrastructure;

internal class RatingRepository(RatingContext ratingContext) : IRatingRepository
{
    public async Task<Rating[]> Find(int[] ratingIds)
    {
        return await ratingContext.Ratings.Where(r => ratingIds.Contains(r.Id)).ToArrayAsync();
    }

    public async Task<Rating[]> FindAll()
    {
        return await ratingContext.Ratings.ToArrayAsync();
    }

    public async Task Add(Rating[] ratings)
    {
        await ratingContext.AddRangeAsync(ratings);
    }

    public async Task<int> Save()
    {
        return await ratingContext.SaveChangesAsync();
    }
}
