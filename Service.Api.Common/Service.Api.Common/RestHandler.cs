using System.Text.Json;
using System.Text.Json.Nodes;
using NATS.Net;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

internal class RestHandler : IRestHandler
{
    public async Task HandlePost<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IPostHandler<TRequest, TResponse> postHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        var request = ProcessRequest(requestData, pathParts, requestMerger);

        var responseBody = await postHandler.Handle(request);

        await Publish(client, requestData, responseBody, 201, cancellationToken);
    }

    public async Task HandlePost<TRequest>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IPostHandler<TRequest> postHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        var request = ProcessRequest(requestData, pathParts, requestMerger);

        await postHandler.Handle(request);

        await Publish(client, requestData, new JsonObject(), 201, cancellationToken);
    }

    public async Task HandleGet<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IGetHandler<TRequest, TResponse> getHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        var request = ProcessRequest(requestData, pathParts, requestMerger);

        var responseBody = await getHandler.Handle(request);

        await Publish(client, requestData, responseBody, 200, cancellationToken);
    }

    public async Task HandleDelete<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IDeleteHandler<TRequest, TResponse> deleteHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        var request = ProcessRequest(requestData, pathParts, requestMerger);

        var responseBody = await deleteHandler.Handle(request);

        await Publish(client, requestData, responseBody, 200, cancellationToken);
    }

    public async Task HandleDelete<TRequest>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IDeleteHandler<TRequest> deleteHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken)
    {
        var request = ProcessRequest(requestData, pathParts, requestMerger);

        await deleteHandler.Handle(request);

        await Publish(client, requestData, new JsonObject(), 204, cancellationToken);
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
