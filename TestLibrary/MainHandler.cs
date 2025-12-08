using System.Text.Json.Nodes;
using AsyncApiBindingsGenerator;
using NATS.Client.JetStream;
using NATS.Net;

namespace TestLibrary.Application.Handlers;

[AsyncApiBindingsMain]
internal partial class MainHandler
{
    private Task HandlePost<TRequest, TResponse>(NatsClient client, NatsJSMsg<Request<JsonNode>> message, IPostHandler<TRequest, TResponse> postHandler, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private Task HandleGet<TResponse>(NatsClient client, NatsJSMsg<Request<JsonNode>> message, IGetHandler<TResponse> getHandler, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
