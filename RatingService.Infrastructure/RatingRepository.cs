using Microsoft.EntityFrameworkCore;
using RatingService.Domain;

namespace RatingService.Infrastructure;

internal class RatingRepository(RatingContext ratingContext) : IRatingRepository
{
    public IAsyncEnumerable<Rating> Find(IEnumerable<Guid> ratingIds)
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

    public async Task<int> Delete(IEnumerable<Guid> ratingIds)
    {
        return await ratingContext.Ratings.Where(r => ratingIds.Contains(r.Id)).ExecuteDeleteAsync();
    }

    public async Task<int> DeleteAll()
    {
        return await ratingContext.Ratings.ExecuteDeleteAsync();
    }

    public async Task<int> Save()
    {
        return await ratingContext.SaveChangesAsync();
    }
}
