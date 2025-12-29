using NRedisStack.Search;

namespace RatingService.Infrastructure.RedisLoadParameters;

internal interface ILoadParameters
{
    public FieldName[] Value { get; }
}