using NRedisStack.Search;

namespace RatingService.Infrastructure.RedisLoadParameters;

internal readonly record struct JsonSurrogateKey : ILoadParameters
{
    private static readonly FieldName[] value = ["$", "__key"];

    public FieldName[] Value => value;
}
