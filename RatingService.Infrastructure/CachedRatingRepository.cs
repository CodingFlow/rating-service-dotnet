using NRedisStack.Json.DataTypes;
using NRedisStack.RedisStackCommands;
using RatingService.Domain;
using RatingService.Infrastructure.Redis;
using RatingService.Infrastructure.RedisLoadParameters;
using RatingService.Infrastructure.RedisQueries;
using StackExchange.Redis;

namespace RatingService.Infrastructure;

internal class CachedRatingRepository(IRatingRepository ratingRepository, IRedisGetConsistent redisGet, IRedisConnection redisConnection, IRedisContext ratingRedisContext) : RatingRepositoryDecorator(ratingRepository)
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
        var redisInput = ratings.Select((item) => new KeyPathValue($"rating:{item.Id}", "$", item)).ToArray();

        var redisTask = redisConnection.Database.JSON().MSetAsync(redisInput);

        var databaseTask = base.Add(ratings);

        await Task.WhenAll(redisTask, databaseTask);
    }

    public override async Task<int> Delete(IEnumerable<Guid> ratingIds)
    {
        var results = ratingRedisContext.Search(new ByIds(ratingIds), new JsonSurrogateKey(), "idx:ratings");
        var surrogateKeys = results
            .Select(row => row["__key"])
            .Select(value => new RedisKey(value));

        var keysToDelete = new List<RedisKey>();

        await foreach (var surrogateKey in surrogateKeys)
        {
            keysToDelete.Add(surrogateKey);
        }

        var redisTask = redisConnection.Database.KeyDeleteAsync([.. keysToDelete]);

        var databaseTask = base.Delete(ratingIds);

        await Task.WhenAll(redisTask, databaseTask);

        return await databaseTask;
    }

    public override Task<int> DeleteAll()
    {
        return base.DeleteAll();
    }
}
