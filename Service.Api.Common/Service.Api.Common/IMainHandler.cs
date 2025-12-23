using System.Text.Json.Nodes;
using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common;

internal interface IMainHandler
{
    Task HandleRequest(NatsClient client, string[] pathParts, INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken);
}