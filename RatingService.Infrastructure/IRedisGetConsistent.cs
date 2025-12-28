namespace RatingService.Infrastructure;

internal interface IRedisGetConsistent
{
    IAsyncEnumerable<T> GetCached<T>(Func<IAsyncEnumerable<T>> fallbackSource);
    IAsyncEnumerable<T> GetCached<Guid, T>(IEnumerable<Guid> targetIds, Func<IAsyncEnumerable<T>> fallbackSource);
}