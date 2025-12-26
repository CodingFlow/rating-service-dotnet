using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NATS.Net;

namespace Service.Api.Common;

internal class Main(
    IOptions<NatsServiceOptions> natsServiceOptions,
    IOptions<ServiceStreamConsumerOptions> serviceStreamConsumerOptions,
    IMemoryCache memoryCache,
    IServiceScopeFactory serviceScopeFactory) : IHostedService
{
    private readonly NatsServiceOptions natsServiceSettings = natsServiceOptions.Value;
    private readonly ServiceStreamConsumerOptions serviceStreamConsumerSettings = serviceStreamConsumerOptions.Value;
    private NatsClient? client;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var host = natsServiceSettings.ServiceHost;
        var port = natsServiceSettings.Port;

        Console.WriteLine($"~~ ~~ Connecting to NATS at {host}:{port}");

        var url = $"nats://{host}:{port}";

        client = new NatsClient(url);

        var jetStream = client.CreateJetStreamContext();
        var consumer = await jetStream.GetConsumerAsync(serviceStreamConsumerSettings.StreamName, serviceStreamConsumerSettings.ConsumerName, cancellationToken);
        
        Console.WriteLine("Ready to process messages");

        var messages = consumer.ConsumeAsync<Request<JsonNode>>(cancellationToken: cancellationToken);

        await Parallel.ForEachAsync(messages, async (message, cancellationToken) =>
        {
            message.Headers.TryGetValue("Nats-Msg-Id", out var messageId);
            var messageAlreadyReceived = memoryCache.TryGetValue(messageId, out _);
            if (!messageAlreadyReceived)
            {
                var memoryCacheOptions = new MemoryCacheEntryOptions { 
                    SlidingExpiration = TimeSpan.FromSeconds(1)
                };
                memoryCache.Set(messageId, true, memoryCacheOptions);
                await using var scope = serviceScopeFactory.CreateAsyncScope();
                var messageHandler = scope.ServiceProvider.GetService<IMessageHandler>();
                await messageHandler.HandleMessage(client, message, cancellationToken);
            } else
            {
                Console.WriteLine($"Ignored duplicate message with id {messageId}");
            }
        });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client!.DisposeAsync();
    }
}
