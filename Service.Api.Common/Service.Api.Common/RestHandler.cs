using System.Text.Json;
using System.Text.Json.Nodes;
using NATS.Net;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

internal class RestHandler : IRestHandler
{
    public async Task HandlePost<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> postHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        await HandleRequest(201, client, requestData, pathParts, postHandler, requestMerger, cancellationToken);
    }

    public async Task HandlePost<TRequest>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest> postHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        await HandleRequest(201, client, requestData, pathParts, postHandler, requestMerger, cancellationToken);
    }

    public async Task HandleGet<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> getHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        await HandleRequest(200, client, requestData, pathParts, getHandler, requestMerger, cancellationToken);
    }

    public async Task HandleDelete<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> deleteHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        await HandleRequest(200, client, requestData, pathParts, deleteHandler, requestMerger, cancellationToken);
    }

    public async Task HandleDelete<TRequest>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest> deleteHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        await HandleRequest(204, client, requestData, pathParts, deleteHandler, requestMerger, cancellationToken);
    }

    private static async Task HandleRequest<TRequest>(int statusCode, NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest> deleteHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        var request = ProcessRequest(requestData, pathParts, requestMerger);

        await deleteHandler.Handle(request);

        await Publish(client, requestData, new JsonObject(), statusCode, cancellationToken);
    }

    private static async Task HandleRequest<TRequest, TResponse>(int statusCode, NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> deleteHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        var request = ProcessRequest(requestData, pathParts, requestMerger);

        var responseBody = await deleteHandler.Handle(request);

        await Publish(client, requestData, responseBody, statusCode, cancellationToken);
    }

    private static TRequest? ProcessRequest<TRequest>(Request<JsonNode> requestData, string[] pathParts, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger)
    {
        var requestBody = requestData.Body.Deserialize<TRequest>();
        var mergedRequest = requestMerger(requestBody, requestData.QueryParameters, pathParts);
        
        return mergedRequest;
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
