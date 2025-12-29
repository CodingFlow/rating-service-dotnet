namespace RatingService.Infrastructure.RedisQueries;

internal interface IRedisQuery
{
    string Value { get; }
}