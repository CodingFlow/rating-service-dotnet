using RatingService.Domain;
using RatingService.Infrastructure.Redis;

namespace RatingService.Infrastructure;

internal class CachedRatingReadOnlyRepository(IRatingReadOnlyRepository ratingReadOnlyRepository, IRedisGetConsistent redisGet) : RatingReadOnlyRepositoryDecorator(ratingReadOnlyRepository)
{
    public override IAsyncEnumerable<Rating> Find(IEnumerable<Guid> ratingIds)
    {
        return redisGet.GetCached(ratingIds, () => base.Find(ratingIds));
    }

    public override IAsyncEnumerable<Rating> FindAll()
    {
        return redisGet.GetCached(base.FindAll);
    }
}
