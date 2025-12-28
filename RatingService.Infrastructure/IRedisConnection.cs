using StackExchange.Redis;

namespace RatingService.Infrastructure;

internal interface IRedisConnection
{
    IDatabase Database { get; }
}