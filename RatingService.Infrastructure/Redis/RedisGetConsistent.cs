using System.Text.Json;
using RatingService.Infrastructure.RedisLoadParameters;
using RatingService.Infrastructure.RedisQueries;
using StackExchange.Redis;

namespace RatingService.Infrastructure.Redis;

internal class RedisGetConsistent(IRedisContext context) : IRedisGetConsistent
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
            Console.WriteLine($"~ ~ query: {query.Value}");
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
