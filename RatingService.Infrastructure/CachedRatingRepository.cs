using RatingService.Domain;
using RatingService.Infrastructure.Redis;

namespace RatingService.Infrastructure;

internal class CachedRatingRepository(IRatingRepository ratingRepository, IRedisGetConsistent redisGet, IRedisContext redisContext) : RatingRepositoryDecorator(ratingRepository)
{
    public override IAsyncEnumerable<Rating> Find(IEnumerable<Guid> ratingIds)
    {
        return redisGet.GetCached(() => base.Find(ratingIds));
    }

    public override IAsyncEnumerable<Rating> FindAll()
    {
        return redisGet.GetCached(base.FindAll);
    }

    public override async Task Add(IEnumerable<Rating> ratings)
    {
        var redisTask = redisContext.Set(ratings, (rating) => rating.Id, "rating:");

        var databaseTask = base.Add(ratings);

        await Task.WhenAll(redisTask, databaseTask);
    }

    public override async Task<int> Delete(IEnumerable<Guid> ratingIds)
    {
        var redisTask = redisContext.Delete(ratingIds, "idx:ratings");

        var databaseTask = base.Delete(ratingIds);

        await Task.WhenAll(redisTask, databaseTask);

        return await databaseTask;
    }

    public override async Task<int> DeleteAll()
    {
        var redisTask = redisContext.DeleteAll("rating:");

        var databaseTask = base.DeleteAll();

        await Task.WhenAll(redisTask, databaseTask);

        return await databaseTask;
    }
}
