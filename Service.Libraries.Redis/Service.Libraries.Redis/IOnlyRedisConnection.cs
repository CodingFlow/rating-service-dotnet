using StackExchange.Redis;

namespace Service.Libraries.Redis;

internal interface IOnlyRedisConnection
{
    IConnectionMultiplexer Connection { get; }
}