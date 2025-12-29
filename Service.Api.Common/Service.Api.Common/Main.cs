using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NATS.Client.JetStream;
using NATS.Net;
using Service.Abstractions;
using Service.Libraries.Redis;
using StackExchange.Redis;

namespace Service.Api.Common;

internal class Main(
    IOptions<NatsServiceOptions> natsServiceOptions,
    IOptions<ServiceStreamConsumerOptions> serviceStreamConsumerOptions,
    IMemoryCache memoryCache,
    IServiceScopeFactory serviceScopeFactory,
    IRedisConnection redisConnection,
    IEnumerable<IStartupService> startupServices) : IHostedService
{
    private readonly NatsServiceOptions natsServiceSettings = natsServiceOptions.Value;
    private readonly ServiceStreamConsumerOptions serviceStreamConsumerSettings = serviceStreamConsumerOptions.Value;
    private NatsClient? client;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var startupTasks = startupServices.Select(service => service.Startup());

        var host = natsServiceSettings.ServiceHost;
        var port = natsServiceSettings.Port;

        Console.WriteLine($"~~ ~~ Connecting to NATS at {host}:{port}");

        var url = $"nats://{host}:{port}";

        client = new NatsClient(url);

        var jetStream = client.CreateJetStreamContext();
        var consumer = await jetStream.GetConsumerAsync(serviceStreamConsumerSettings.StreamName, serviceStreamConsumerSettings.ConsumerName, cancellationToken);

        await Task.WhenAll(startupTasks);
        
        Console.WriteLine("Ready to process messages");

        var messages = consumer.ConsumeAsync<Request<JsonNode>>(cancellationToken: cancellationToken);

        await Parallel.ForEachAsync(messages, async (message, cancellationToken) =>
        {
            var isInLocalCache = CheckLocalCache(message);
            var isInDistributedCache = await CheckDistributedCache(message, isInLocalCache);

            if (!isInLocalCache && !isInDistributedCache)
            {
                await BeginRequestScope(message, cancellationToken);
            }
        });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client!.DisposeAsync();
    }

    private bool CheckLocalCache(INatsJSMsg<Request<JsonNode>> message)
    {
        message.Headers.TryGetValue("Nats-Msg-Id", out var messageId);

        var messageAlreadyReceived = memoryCache.TryGetValue(messageId, out _);

        if (!messageAlreadyReceived)
        {
            var memoryCacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(1)
            };

            memoryCache.Set(messageId, true, memoryCacheOptions);
        }
        else
        {
            Console.WriteLine($"Ignored duplicate message with id {messageId} because it is in local cache.");
        }

        return messageAlreadyReceived;
    }

    private async Task<bool> CheckDistributedCache(INatsJSMsg<Request<JsonNode>> message, bool isInLocalCache)
    {
        if (isInLocalCache)
        {
            return true;
        }

        message.Headers.TryGetValue("Nats-Msg-Id", out var messageId);

        var value = await redisConnection.Database.StringGetAsync(messageId.ToString());

        var messageAlreadyReceived = value.HasValue;

        if (!messageAlreadyReceived)
        {
            await redisConnection.Database.StringSetAsync(
                messageId.ToString(),
                "PENDING",
                expiry: TimeSpan.FromSeconds(2),
                When.NotExists);
        }
        else
        {
            Console.WriteLine($"Ignored duplicate message with id {messageId} because it is in distributed cache.");
        }

        return messageAlreadyReceived;
    }

    private async Task BeginRequestScope(INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var messageHandler = scope.ServiceProvider.GetService<IMessageHandler>();

        await messageHandler.HandleMessage(client, message, cancellationToken);
    }
}
