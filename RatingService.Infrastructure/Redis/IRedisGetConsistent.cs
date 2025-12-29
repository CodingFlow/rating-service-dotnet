using NRedisStack.Search;

namespace RatingService.Infrastructure.Redis;

internal interface IRedisGetConsistent
{
    IAsyncEnumerable<T> GetCached<T>(Func<IAsyncEnumerable<T>> fallbackSource);
    IAsyncEnumerable<T> GetCached<T>(IEnumerable<Guid> targetIds, Func<IAsyncEnumerable<T>> fallbackSource);
}