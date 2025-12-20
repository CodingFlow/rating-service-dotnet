using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common;

internal class Main(
    IOptions<NatsServiceOptions> natsServiceOptions,
    IOptions<ServiceStreamConsumerOptions> serviceStreamConsumerOptions,
    IServiceScopeFactory serviceScopeFactory) : IHostedService
{
    private readonly NatsServiceOptions natsServiceSettings = natsServiceOptions.Value;
    private readonly ServiceStreamConsumerOptions serviceStreamConsumerSettings = serviceStreamConsumerOptions.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Run();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task Run()
    {
        var host = natsServiceSettings.ServiceHost;
        var port = natsServiceSettings.Port;

        Console.WriteLine($"~~ ~~ Connecting to NATS at {host}:{port}");

        var url = $"nats://{host}:{port}";

        var client = new NatsClient(url);

        var jetStream = client.CreateJetStreamContext();
        var consumer = await jetStream.GetConsumerAsync(serviceStreamConsumerSettings.StreamName, serviceStreamConsumerSettings.ConsumerName);

        Console.WriteLine("Ready to process messages");

        var messages = consumer.ConsumeAsync<Request<JsonNode>>();

        await Parallel.ForEachAsync(messages, async (message, cancellationToken) =>
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var mainHandler = scope.ServiceProvider.GetService<IMainHandler>();
            await HandleMessage(client, message, mainHandler, cancellationToken);
        });
    }

    private static async ValueTask HandleMessage(NatsClient client, NatsJSMsg<Request<JsonNode>> message, IMainHandler mainHandler, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("processing message");
            var splitSubject = ExtractHttpMethod(message);

            Console.WriteLine($"Data: {JsonSerializer.Serialize(message.Data)}");

            await mainHandler.HandleRequest(client, splitSubject, message, cancellationToken);

            await message.AckAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex}");
        }
    }

    private static (string httpMethod, string pathPart) ExtractHttpMethod<T>(NatsJSMsg<Request<T>> message)
    {
        var firstPartIndex = message.Subject.IndexOf('.');
        var secondPartIndex = message.Subject.IndexOf('.', firstPartIndex + 1);

        if (secondPartIndex == -1)
        {
            secondPartIndex = message.Subject.Length;
        }

        Console.WriteLine($"first index: {firstPartIndex} -- second index: {secondPartIndex}");

        return (message.Subject[..firstPartIndex], message.Subject[(firstPartIndex + 1)..secondPartIndex]);
    }
}
