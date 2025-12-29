using Service.Libraries.Redis;

namespace RatingService.Infrastructure.Redis;

internal class RedisWriteThrough(IRedisContext context) : IRedisWriteThrough
{
    public async Task Add<T>(IEnumerable<T> items, Func<T, Guid> getId, string prefix, Func<IEnumerable<T>, Task> databaseGet)
        where T : notnull
    {
        var redisTask = context.Set(items, getId, prefix);

        var databaseTask = databaseGet(items);

        await Task.WhenAll(redisTask, databaseTask);
    }

    public async Task<int> Delete(IEnumerable<Guid> ids, string index, Func<IEnumerable<Guid>, Task<int>> databaseDelete)
    {
        var redisTask = context.Delete(ids, index);

        var databaseTask = databaseDelete(ids);

        await Task.WhenAll(redisTask, databaseTask);

        return await databaseTask;
    }

    public async Task<int> DeleteAll(string prefix, Func<Task<int>> databaseDeleteAll)
    {
        var redisTask = context.DeleteAll(prefix);

        var databaseTask = databaseDeleteAll();

        await Task.WhenAll(redisTask, databaseTask);

        return await databaseTask;
    }
}
