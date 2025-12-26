using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Memory;
using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common;

internal class MessageHandler(IMemoryCache memoryCache, IRequestDispatcher requestDispatcher) : IMessageHandler
{
    public async ValueTask HandleMessage(NatsClient client, INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("processing message");
            Console.WriteLine($"Headers: {JsonSerializer.Serialize(message.Headers)}");
            Console.WriteLine($"Data: {JsonSerializer.Serialize(message.Data)}");

            var pathParts = ExtractPathParts(message);

            await requestDispatcher.DispatchRequest(client, pathParts, message.Data, cancellationToken);

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

    private static string[] ExtractPathParts(INatsJSMsg<Request<JsonNode>> message)
    {
        return message.Subject.Split('.');
    }
}
