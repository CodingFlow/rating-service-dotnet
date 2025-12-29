using Microsoft.Extensions.Options;
using Service.Abstractions;
using StackExchange.Redis;

namespace Service.Libraries.Redis;

internal class RedisConnection(IOptions<RedisOptions> redisOptions) : IStartupService, IRedisConnection
{
    private readonly RedisOptions redisSettings = redisOptions.Value;

    public IDatabase Database { get; private set; }
    public IServer Server { get; private set; }

    public async Task Startup()
    {
        var muxer = ConnectionMultiplexer.Connect($"{redisSettings.Host}:{redisSettings.Port}");
        Database = muxer.GetDatabase();
        Server = muxer.GetServer(muxer.GetEndPoints().First());
    }
}
