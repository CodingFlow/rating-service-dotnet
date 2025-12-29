using NRedisStack.Json.DataTypes;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using RatingService.Infrastructure.RedisLoadParameters;
using RatingService.Infrastructure.RedisQueries;
using StackExchange.Redis;

namespace RatingService.Infrastructure.Redis;

internal class RedisContext(IRedisConnection connection) : IRedisContext
{
    public IAsyncEnumerable<Row> Search<T, TLoadParameters>(T query, TLoadParameters loadParameters, string index)
        where T : IRedisQuery
        where TLoadParameters : ILoadParameters
    {
        var request = new AggregationRequest(query.Value).Load(loadParameters.Value).Dialect(2);
        var results = connection.Database.FT().AggregateAsyncEnumerable(index, request);

        return results;
    }

    public async Task<bool> Set<T>(IEnumerable<T> items, Func<T, Guid> getId, string prefix)
        where T : notnull
    {
        var redisInput = items.Select((item) => new KeyPathValue($"{prefix}{getId(item)}", "$", item));

        return await connection.Database.JSON().MSetAsync([.. redisInput]);
    }

    public async Task<long> Delete(IEnumerable<Guid> ids, string index)
    {
        var results = Search(new ByIds(ids), new JsonSurrogateKey(), index);
        
        var surrogateKeys = results
            .Select(row => row["__key"])
            .Select(value => new RedisKey(value));

        var keysToDelete = new List<RedisKey>();

        await foreach (var surrogateKey in surrogateKeys)
        {
            keysToDelete.Add(surrogateKey);
        }

        return await connection.Database.KeyDeleteAsync([.. keysToDelete]);
    }

    public async Task<long> DeleteAll(string prefix)
    {
        var keys = connection.Server.KeysAsync(pattern: $"{prefix}*");
        var keysToDelete = await keys.ToArrayAsync();

        return await connection.Database.KeyDeleteAsync(keysToDelete);
    }
}
