using System.Diagnostics;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NATS.Client.JetStream;
using NATS.Net;
using Service.Abstractions;
using Service.Libraries.Redis;
using StackExchange.Redis;

namespace Service.Api.Common;

internal partial class Main(
    IOptions<NatsServiceOptions> natsServiceOptions,
    IOptions<ServiceStreamConsumerOptions> serviceStreamConsumerOptions,
    IMemoryCache memoryCache,
    IServiceScopeFactory serviceScopeFactory,
    IRedisConnection redisConnection,
    IEnumerable<IStartupService> startupServices,
    ILogger<Main> logger) : IHostedService
{
    private readonly NatsServiceOptions natsServiceSettings = natsServiceOptions.Value;
    private readonly ServiceStreamConsumerOptions serviceStreamConsumerSettings = serviceStreamConsumerOptions.Value;
    private NatsClient? client;
    private static readonly ActivitySource tracer = new ActivitySource(nameof(Main));

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var startupTasks = startupServices.Select(service => service.Startup());

        var consumer = await ConnectToNats(cancellationToken);

        await Task.WhenAll(startupTasks);

        LogReadyMessages();

        var messages = consumer.ConsumeAsync<Request<JsonNode>>(cancellationToken: cancellationToken);

        await Parallel.ForEachAsync(messages, async (message, cancellationToken) =>
        {
            message.Headers.TryGetValue("Nats-Msg-Id", out var messageId);

            var state = new Dictionary<string, object>() {
                { "NatsMsgId", messageId }
            };

            using (logger.BeginScope(state))
            using (tracer.StartActivity("Request"))
            {
                var isInLocalCache = CheckLocalCache(messageId);
                var isInDistributedCache = await CheckDistributedCache(messageId, isInLocalCache);

                if (!isInLocalCache && !isInDistributedCache)
                {
                    await BeginRequestScope(message, cancellationToken);
                }
            }
        });
    }

    private async Task<INatsJSConsumer> ConnectToNats(CancellationToken cancellationToken)
    {
        var host = natsServiceSettings.ServiceHost;
        var port = natsServiceSettings.Port;

        LogConnectedToNats(host, port);

        var url = $"nats://{host}:{port}";

        client = new NatsClient(url);

        var jetStream = client.CreateJetStreamContext();
        var consumer = await jetStream.GetConsumerAsync(serviceStreamConsumerSettings.StreamName, serviceStreamConsumerSettings.ConsumerName, cancellationToken);
        
        return consumer;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client!.DisposeAsync();
    }

    private bool CheckLocalCache(StringValues messageId)
    {
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
            LogIgnoreDuplicateMessageLocal(messageId);
        }

        return messageAlreadyReceived;
    }

    private async Task<bool> CheckDistributedCache(StringValues messageId, bool isInLocalCache)
    {
        if (isInLocalCache)
        {
            return true;
        }

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
            LogIgnoreDuplicateMessageDistributed(messageId);
        }

        return messageAlreadyReceived;
    }

    private async Task BeginRequestScope(INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var messageHandler = scope.ServiceProvider.GetService<IMessageHandler>();

        await messageHandler.HandleMessage(client, message, cancellationToken);
    }

    [LoggerMessage(LogLevel.Information, Message = "Connecting to NATS at {host}:{port}")]
    private partial void LogConnectedToNats(string host, string port);

    [LoggerMessage(LogLevel.Information, Message = "Ready to process messages")]
    private partial void LogReadyMessages();

    [LoggerMessage(LogLevel.Information, Message = "Ignored duplicate message with id {messageId} because it is in local cache.")]
    private partial void LogIgnoreDuplicateMessageLocal(string messageId);

    [LoggerMessage(LogLevel.Information, Message = "Ignored duplicate message with id {messageId} because it is in distributed cache.")]
    private partial void LogIgnoreDuplicateMessageDistributed(string messageId);
}
