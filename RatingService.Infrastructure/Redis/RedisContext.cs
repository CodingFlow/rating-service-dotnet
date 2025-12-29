using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using RatingService.Infrastructure.RedisLoadParameters;
using RatingService.Infrastructure.RedisQueries;

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
}
