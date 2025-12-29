
namespace RatingService.Infrastructure.Redis;

internal interface IRedisWriteThrough
{
    Task Add<T>(IEnumerable<T> items, Func<T, Guid> getId, string prefix, Func<IEnumerable<T>, Task> databaseGet) where T : notnull;
    Task<int> Delete(IEnumerable<Guid> ids, string index, Func<IEnumerable<Guid>, Task<int>> databaseDelete);
    Task<int> DeleteAll(string prefix, Func<Task<int>> databaseDeleteAll);
}