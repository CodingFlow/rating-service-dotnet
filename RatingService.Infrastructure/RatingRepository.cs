using Microsoft.EntityFrameworkCore;
using RatingService.Domain;

namespace RatingService.Infrastructure;

internal class RatingRepository(RatingContext ratingContext) : IRatingRepository
{
    public IAsyncEnumerable<Rating> Find(int[] ratingIds)
    {
        return ratingContext.Ratings.Where(r => ratingIds.Contains(r.Id)).AsNoTracking().AsAsyncEnumerable();
    }

    public IAsyncEnumerable<Rating> FindAll()
    {
        return ratingContext.Ratings.AsNoTracking().AsAsyncEnumerable();
    }

    public async Task Add(IEnumerable<Rating> ratings)
    {
        await ratingContext.AddRangeAsync(ratings);
    }

    public async Task<int> Save()
    {
        return await ratingContext.SaveChangesAsync();
    }
}
