using NRedisStack.Search.Aggregation;
using RatingService.Infrastructure.RedisLoadParameters;
using RatingService.Infrastructure.RedisQueries;

namespace RatingService.Infrastructure.Redis;

internal interface IRedisContext
{
    IAsyncEnumerable<Row> Search<T, TLoadParameters>(T query, TLoadParameters loadParameters, string index)
        where T : IRedisQuery
        where TLoadParameters : ILoadParameters;
}