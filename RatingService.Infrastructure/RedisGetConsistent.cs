using System.Text.Json;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using StackExchange.Redis;

namespace RatingService.Infrastructure;

internal class RedisGetConsistent(IRedisConnection redisConnection) : IRedisGetConsistent
{
    public IAsyncEnumerable<T> GetCached<T>(Func<IAsyncEnumerable<T>> fallbackSource)
    {
        var query = "*";

        return GetData(query, fallbackSource);
    }

    public IAsyncEnumerable<T> GetCached<Guid, T>(IEnumerable<Guid> targetIds, Func<IAsyncEnumerable<T>> fallbackSource)
    {
        var escapedIds = targetIds.Select(id => id.ToString().Replace("-", @"\-"));
        var joinedIds = string.Join(" | ", escapedIds);
        var query = $@"@id:{{{joinedIds}}}";

        return GetData(query, fallbackSource);
    }

    private IAsyncEnumerable<T> GetData<T>(string query, Func<IAsyncEnumerable<T>> fallbackSource)
    {
        try
        {
            var request = new AggregationRequest(query).Load("$").Dialect(2);

            var results = redisConnection.Database.FT().AggregateAsyncEnumerable("idx:ratings", request);

            var items = results
                .Select(row => row.GetString("$"))
                .Where(json => !string.IsNullOrEmpty(json))
                .Select(json => JsonSerializer.Deserialize<T>(json!));

            return items!;
        }
        catch (RedisException)
        {
            return fallbackSource();
        }
    }
}
