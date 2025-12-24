using System.Text.Json;
using System.Text.Json.Nodes;
using NATS.Net;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

internal class RestHandler : IRestHandler
{
    public async Task HandlePost<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IPostHandler<TRequest, TResponse> postHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        var requestBody = requestData.Body.Deserialize<TRequest>();
        var mergedRequest = requestMerger(requestBody, requestData.QueryParameters, pathParts);
        
        var responseBody = await postHandler.Handle(mergedRequest);

        await Publish(client, requestData, responseBody, 201, cancellationToken);
    }

    public async Task HandlePost<TRequest>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IPostHandler<TRequest> postHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        var requestBody = requestData.Body.Deserialize<TRequest>();
        var mergedRequest = requestMerger(requestBody, requestData.QueryParameters, pathParts);
        
        await postHandler.Handle(mergedRequest);

        await Publish(client, requestData, string.Empty, 201, cancellationToken);
    }

    public async Task HandleGet<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IGetHandler<TRequest, TResponse> getHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        var requestBody = requestData.Body.Deserialize<TRequest>();
        var mergedRequest = requestMerger(requestBody, requestData.QueryParameters, pathParts);
        
        var responseBody = await getHandler.Handle(mergedRequest);

        await Publish(client, requestData, responseBody, 200, cancellationToken);
    }

    public async Task HandleGet<TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IGetHandler<TResponse> getHandler, CancellationToken cancellationToken)
    {
        var responseBody = await getHandler.Handle();
        
        await Publish(client, requestData, responseBody, 200, cancellationToken);
    }

    private static async Task Publish<TResponse>(NatsClient client, Request<JsonNode> requestData, TResponse? responseBody, int statusCode, CancellationToken cancellationToken)
    {
        await client.PublishAsync(requestData.OriginReplyTo, new Response<TResponse>
        {
            StatusCode = statusCode,
            Body = responseBody,
            Headers = []
        },
                cancellationToken: cancellationToken);
    }
}
