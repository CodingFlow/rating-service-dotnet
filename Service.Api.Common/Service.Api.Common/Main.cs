using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NATS.Client.JetStream;
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
                var mainHandler = scope.ServiceProvider.GetService<IMainHandler>();
                await HandleMessage(client, message, mainHandler, cancellationToken);
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

    private async ValueTask HandleMessage(NatsClient client, INatsJSMsg<Request<JsonNode>> message, IMainHandler mainHandler, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("processing message");
            Console.WriteLine($"Headers: {JsonSerializer.Serialize(message.Headers)}");
            Console.WriteLine($"Data: {JsonSerializer.Serialize(message.Data)}");

            var splitSubject = ExtractHttpMethod(message);
            var pathParts = ExtractRemaingPathParts(message);

            Console.WriteLine($"httpMethod: {splitSubject.httpMethod} -- pathPart: {splitSubject.pathPart}");

            await mainHandler.HandleRequest(client, splitSubject, pathParts, message, cancellationToken);

            await message.AckAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex}");

            message.Headers.TryGetValue("Nats-Msg-Id", out var messageId);
            memoryCache.Remove(messageId);

            await message.NakAsync(cancellationToken: cancellationToken);
        }
    }

    private static (string httpMethod, string pathPart) ExtractHttpMethod<T>(INatsJSMsg<Request<T>> message)
    {
        var firstPartIndex = message.Subject.IndexOf('.');
        var secondPartIndex = message.Subject.IndexOf('.', firstPartIndex + 1);

        if (secondPartIndex == -1)
        {
            secondPartIndex = message.Subject.Length;
        }

        return (message.Subject[..firstPartIndex], message.Subject[(firstPartIndex + 1)..secondPartIndex]);
    }

    private static string[] ExtractRemaingPathParts(INatsJSMsg<Request<JsonNode>> message)
    {
        var firstPartIndex = message.Subject.IndexOf('.');
        var secondPartIndex = message.Subject.IndexOf('.', firstPartIndex + 1);

        if (secondPartIndex == -1)
        {
            secondPartIndex = message.Subject.Length;
        }

        var remainingParts = message.Subject.Substring(secondPartIndex);

        return remainingParts.Split('.');
    }
}
