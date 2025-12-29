using NRedisStack.Search.Aggregation;
using Service.Libraries.Redis.RedisLoadParameters;
using Service.Libraries.Redis.RedisQueries;

namespace Service.Libraries.Redis;

public interface IRedisContext
{
    IAsyncEnumerable<Row> Search<T, TLoadParameters>(T query, TLoadParameters loadParameters, string index)
        where T : IRedisQuery
        where TLoadParameters : ILoadParameters;
    Task<bool> Set<T>(IEnumerable<T> items, Func<T, Guid> getId, string prefix) where T : notnull;
    Task<long> Delete(IEnumerable<Guid> ids, string index);
    Task<long> DeleteAll(string prefix);
}