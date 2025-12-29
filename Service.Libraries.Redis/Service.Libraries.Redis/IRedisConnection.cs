using StackExchange.Redis;

namespace Service.Libraries.Redis;

public interface IRedisConnection
{
    IDatabase Database { get; }
    IServer Server { get; }
}