using System.Text.Json.Nodes;
using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common;

internal interface IMainHandler
{
    Task HandleRequest(NatsClient client, (string httpMethod, string pathPart) splitSubject, NatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken);
}