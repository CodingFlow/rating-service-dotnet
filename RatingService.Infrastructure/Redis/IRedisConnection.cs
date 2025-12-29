using StackExchange.Redis;

namespace RatingService.Infrastructure.Redis;

internal interface IRedisConnection
{
    IDatabase Database { get; }
}