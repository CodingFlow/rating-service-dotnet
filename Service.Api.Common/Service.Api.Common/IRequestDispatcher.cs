using System.Text.Json.Nodes;
using NATS.Net;

namespace Service.Api.Common;

public interface IRequestDispatcher
{
    Task DispatchRequest(NatsClient client, string[] pathParts, Request<JsonNode> requestData, CancellationToken cancellationToken);
}