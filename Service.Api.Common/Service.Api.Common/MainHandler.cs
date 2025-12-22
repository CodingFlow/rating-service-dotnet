using System.Text.Json.Nodes;
using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common;

internal class MainHandler(IRequestDispatcher requestDispatcher) : IMainHandler
{
    public async Task HandleRequest(NatsClient client, (string httpMethod, string pathPart) splitSubject, INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"httpMethod: {splitSubject.httpMethod} -- pathPart: {splitSubject.pathPart}");
        await requestDispatcher.DispatchRequest(client, splitSubject, message, new RestHandler(), cancellationToken);
    }
}
