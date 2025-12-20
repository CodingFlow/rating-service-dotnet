using RatingService.Domain;

namespace RatingService.Infrastructure;

internal class RatingRepository(RatingContext ratingContext) : IRatingRepository
{
    public async Task Add(Rating[] ratings)
    {
        await ratingContext.AddRangeAsync(ratings);
    }

    public async Task<int> Save()
    {
        return await ratingContext.SaveChangesAsync();
    }
}
