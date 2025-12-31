using Microsoft.Extensions.Options;
using OpenTelemetry.Instrumentation.StackExchangeRedis;
using Service.Abstractions;
using StackExchange.Redis;

namespace Service.Libraries.Redis;

internal class RedisConnection(IOptions<RedisOptions> redisOptions, StackExchangeRedisInstrumentation instrumentation) : IStartupService, IRedisConnection
{
    private readonly RedisOptions redisSettings = redisOptions.Value;
    public IDatabase Database { get; private set; }
    public IServer Server { get; private set; }

    public async Task Startup()
    {
        var muxer = await ConnectionMultiplexer.ConnectAsync($"{redisSettings.Host}:{redisSettings.Port}");

        Database = muxer.GetDatabase();
        Server = muxer.GetServer(muxer.GetEndPoints().First());

        instrumentation.AddConnection(muxer);
    }
}
