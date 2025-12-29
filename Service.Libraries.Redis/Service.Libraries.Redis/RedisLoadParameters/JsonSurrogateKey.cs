using NRedisStack.Search;

namespace Service.Libraries.Redis.RedisLoadParameters;

public readonly record struct JsonSurrogateKey : ILoadParameters
{
    private static readonly FieldName[] value = ["$", "__key"];

    public FieldName[] Value => value;
}
