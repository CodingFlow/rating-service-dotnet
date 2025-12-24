using System.Text.Json.Nodes;
using NATS.Net;

namespace Service.Api.Common;

internal interface IMainHandler
{
    Task HandleRequest(NatsClient client, string[] pathParts, Request<JsonNode> requestData, CancellationToken cancellationToken);
}