using NRedisStack.Search;

namespace Service.Libraries.Redis.RedisLoadParameters;

public readonly record struct Json : ILoadParameters
{
    private static readonly FieldName[] value = ["$"];

    public FieldName[] Value => value;
}
