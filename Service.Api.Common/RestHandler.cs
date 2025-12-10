using System.Text.Json;
using System.Text.Json.Nodes;
using NATS.Client.JetStream;
using NATS.Net;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

internal class RestHandler : IRestHandler
{
    public async Task HandlePost<TRequest, TResponse>(NatsClient client, NatsJSMsg<Request<JsonNode>> message, IPostHandler<TRequest, TResponse> postHandler, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Received request body: {message.Data.Body}");
        var requestBody = message.Data.Body.Deserialize<TRequest>();
        var responseBodyPost = postHandler.Handle(requestBody);

        await client.PublishAsync(message.Data.OriginReplyTo, new Response<TResponse>
        {
            StatusCode = 201,
            Body = responseBodyPost,
            Headers = []
        },
        cancellationToken: cancellationToken);
    }

    public async Task HandleGet<TResponse>(NatsClient client, NatsJSMsg<Request<JsonNode>> message, IGetHandler<TResponse> getHandler, CancellationToken cancellationToken)
    {
        var responseBody = getHandler.Handle();
        await client.PublishAsync(message.Data.OriginReplyTo, new Response<TResponse>
        {
            StatusCode = 200,
            Body = responseBody,
            Headers = []
        },
        cancellationToken: cancellationToken);
    }
}
