using RatingService.Domain;
using RatingService.Infrastructure.Redis;

namespace RatingService.Infrastructure;

internal class CachedRatingRepository(IRatingRepository ratingRepository, IRedisReadConsistent redisGet, IRedisWriteThrough redisWriteThrough) : RatingRepositoryDecorator(ratingRepository)
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
        await redisWriteThrough.Add(
            items: ratings,
            getId: (rating) => rating.Id.Value,
            prefix: "rating:",
            databaseGet: base.Add);
    }

    public override async Task<int> Delete(IEnumerable<Guid> ratingIds)
    {
        return await redisWriteThrough.Delete(
            ids: ratingIds,
            index: "idx:ratings",
            databaseDelete: base.Delete);
    }

    public override async Task<int> DeleteAll()
    {
        return await redisWriteThrough.DeleteAll(
            prefix: "rating:",
            databaseDeleteAll: base.DeleteAll);
    }
}
