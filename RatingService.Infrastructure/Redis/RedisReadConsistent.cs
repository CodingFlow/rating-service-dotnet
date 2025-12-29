using System.Text.Json;
using Service.Libraries.Redis;
using Service.Libraries.Redis.RedisLoadParameters;
using Service.Libraries.Redis.RedisQueries;
using StackExchange.Redis;

namespace RatingService.Infrastructure.Redis;

internal class RedisReadConsistent(IRedisContext context) : IRedisReadConsistent
{
    public IAsyncEnumerable<T> GetCached<T>(Func<IAsyncEnumerable<T>> fallbackSource)
    {
        return GetData(new All(), fallbackSource);
    }

    public IAsyncEnumerable<T> GetCached<T>(IEnumerable<Guid> targetIds, Func<IAsyncEnumerable<T>> fallbackSource)
    {
        return GetData(new ByIds(targetIds), fallbackSource);
    }

    private IAsyncEnumerable<T> GetData<TQuery, T>(TQuery query, Func<IAsyncEnumerable<T>> fallbackSource)
        where TQuery : IRedisQuery
    {
        try
        {
            var results = context.Search(query, new Json(), "idx:ratings");

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
