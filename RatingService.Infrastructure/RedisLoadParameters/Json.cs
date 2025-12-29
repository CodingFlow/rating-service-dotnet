using NRedisStack.Search;

namespace RatingService.Infrastructure.RedisLoadParameters;

internal readonly record struct Json : ILoadParameters
{
    private static readonly FieldName[] value = ["$"];

    public FieldName[] Value => value;
}
