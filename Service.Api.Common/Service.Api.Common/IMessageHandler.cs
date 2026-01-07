using System.Text.Json.Nodes;
using DecoratorGenerator;
using NATS.Client.JetStream;

namespace Service.Api.Common;

[Decorate]
internal interface IMessageHandler
{
    ValueTask HandleMessage(INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken);
}