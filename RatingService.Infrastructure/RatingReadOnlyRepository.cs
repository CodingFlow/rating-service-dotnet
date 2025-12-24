using Microsoft.EntityFrameworkCore;
using RatingService.Domain;

namespace RatingService.Infrastructure;

internal class RatingReadOnlyRepository(RatingReadOnlyContext ratingContext) : IRatingReadOnlyRepository
{
    public IAsyncEnumerable<Rating> Find(IEnumerable<int> ratingIds)
    {
        return ratingContext.Ratings.Where(r => ratingIds.Contains(r.Id)).AsNoTracking().AsAsyncEnumerable();
    }

    public IAsyncEnumerable<Rating> FindAll()
    {
        return ratingContext.Ratings.AsNoTracking().AsAsyncEnumerable();
    }
}
