using System.Text.Json.Nodes;
using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common;

public interface IRequestDispatcher
{
    Task DispatchRequest(NatsClient client, string[] pathParts, INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken);
}