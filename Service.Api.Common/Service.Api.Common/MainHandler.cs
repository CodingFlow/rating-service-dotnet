using System.Text.Json.Nodes;
using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common;

internal class MainHandler(IRequestDispatcher requestDispatcher) : IMainHandler
{
    public async Task HandleRequest(NatsClient client, string[] pathParts, INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken)
    {
        await requestDispatcher.DispatchRequest(client, pathParts, message, cancellationToken);
    }
}
