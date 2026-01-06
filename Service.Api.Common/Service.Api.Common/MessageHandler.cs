using System.Text.Json.Nodes;
using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common;

internal partial class MessageHandler(IRequestDispatcher requestDispatcher) : IMessageHandler
{
    public async ValueTask HandleMessage(NatsClient client, INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken)
    {
        var pathParts = ExtractPathParts(message);

        await requestDispatcher.DispatchRequest(client, pathParts, message.Data, cancellationToken);

        await message.AckAsync(cancellationToken: cancellationToken);
    }

    private static string[] ExtractPathParts(INatsJSMsg<Request<JsonNode>> message)
    {
        return message.Subject.Split('.');
    }
}