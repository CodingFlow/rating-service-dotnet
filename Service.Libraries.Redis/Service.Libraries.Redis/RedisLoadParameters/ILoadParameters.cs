using NRedisStack.Search;

namespace Service.Libraries.Redis.RedisLoadParameters;

public interface ILoadParameters
{
    public FieldName[] Value { get; }
}