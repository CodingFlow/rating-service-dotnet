namespace RatingService.Infrastructure.Redis;

internal interface IRedisReadConsistent
{
    IAsyncEnumerable<T> GetCached<T>(Func<IAsyncEnumerable<T>> fallbackSource);
    IAsyncEnumerable<T> GetCached<T>(IEnumerable<Guid> targetIds, Func<IAsyncEnumerable<T>> fallbackSource);
}