using System.Text.Json.Nodes;
using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common;

internal interface IMessageHandler
{
    ValueTask HandleMessage(NatsClient client, INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken);
}