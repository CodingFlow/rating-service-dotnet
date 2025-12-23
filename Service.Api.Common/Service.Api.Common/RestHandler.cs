using System.Text.Json;
using System.Text.Json.Nodes;
using NATS.Client.JetStream;
using NATS.Net;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

internal class RestHandler : IRestHandler
{
    public async Task HandlePost<TRequest, TResponse>(NatsClient client, INatsJSMsg<Request<JsonNode>> message, string[] pathParts, IPostHandler<TRequest, TResponse> postHandler, Func<TRequest, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Received request body: {message.Data.Body}");

        var requestBody = message.Data.Body.Deserialize<TRequest>();
        var mergedRequest = requestMerger(requestBody, pathParts);
        
        var responseBody = await postHandler.Handle(mergedRequest);

        await Publish(client, message, responseBody, 201, cancellationToken);
    }

    public async Task HandlePost<TRequest>(NatsClient client, INatsJSMsg<Request<JsonNode>> message, string[] pathParts, IPostHandler<TRequest> postHandler, Func<TRequest, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Received request body: {message.Data.Body}");

        var requestBody = message.Data.Body.Deserialize<TRequest>();
        var mergedRequest = requestMerger(requestBody, pathParts);
        
        await postHandler.Handle(mergedRequest);

        await Publish(client, message, string.Empty, 201, cancellationToken);
    }

    public async Task HandleGet<TRequest, TResponse>(NatsClient client, INatsJSMsg<Request<JsonNode>> message, string[] pathParts, IGetHandler<TRequest, TResponse> getHandler, Func<TRequest, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        var requestBody = message.Data.Body.Deserialize<TRequest>();
        var mergedRequest = requestMerger(requestBody, pathParts);
        
        var responseBody = await getHandler.Handle(mergedRequest);

        await Publish(client, message, responseBody, 200, cancellationToken);
    }

    public async Task HandleGet<TResponse>(NatsClient client, INatsJSMsg<Request<JsonNode>> message, string[] pathParts, IGetHandler<TResponse> getHandler, CancellationToken cancellationToken)
    {
        var responseBody = await getHandler.Handle();
        
        await Publish(client, message, responseBody, 200, cancellationToken);
    }

    private static async Task Publish<TResponse>(NatsClient client, INatsJSMsg<Request<JsonNode>> message, TResponse? responseBody, int statusCode, CancellationToken cancellationToken)
    {
        await client.PublishAsync(message.Data.OriginReplyTo, new Response<TResponse>
        {
            StatusCode = statusCode,
            Body = responseBody,
            Headers = []
        },
                cancellationToken: cancellationToken);
    }
}
