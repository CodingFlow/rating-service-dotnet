using System.Text.Json.Nodes;
using NATS.Net;

namespace Service.Api.Common;

internal class MainHandler(IRequestDispatcher requestDispatcher) : IMainHandler
{
    public async Task HandleRequest(NatsClient client, string[] pathParts, Request<JsonNode> requestData, CancellationToken cancellationToken)
    {
        await requestDispatcher.DispatchRequest(client, pathParts, requestData, cancellationToken);
    }
}
