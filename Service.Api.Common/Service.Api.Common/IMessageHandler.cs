using System.Text.Json.Nodes;
using DecoratorGenerator;
using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common;

[Decorate]
internal interface IMessageHandler
{
    ValueTask HandleMessage(NatsClient client, INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken);
}