using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using NATS.Net;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

internal class RestHandler : IRestHandler
{
    public async Task HandlePost<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> postHandler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        await HandleRequest(201, client, requestData, pathParts, postHandler, requestMerger, cancellationToken);
    }

    public async Task HandlePost<TRequest>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest> postHandler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        await HandleRequest(201, client, requestData, pathParts, postHandler, requestMerger, cancellationToken);
    }

    public async Task HandleGet<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> getHandler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        await HandleRequest(200, client, requestData, pathParts, getHandler, requestMerger, cancellationToken);
    }

    public async Task HandleDelete<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> deleteHandler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        await HandleRequest(200, client, requestData, pathParts, deleteHandler, requestMerger, cancellationToken);
    }

    public async Task HandleDelete<TRequest>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest> deleteHandler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        await HandleRequest(204, client, requestData, pathParts, deleteHandler, requestMerger, cancellationToken);
    }

    private static async Task HandleRequest<TRequest>(int statusCode, NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest> handler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        var (request, errors) = ProcessRequest(requestData, pathParts, requestMerger);

        if (errors.Any())
        {
            await Publish(client, requestData, errors.First(), (int)errors.First().StatusCode, cancellationToken);
        } else
        {
            await handler.Handle(request);

            await Publish(client, requestData, new JsonObject(), statusCode, cancellationToken);
        }
    }

    private static async Task HandleRequest<TRequest, TResponse>(int statusCode, NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> handler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        var (request, errors) = ProcessRequest(requestData, pathParts, requestMerger);

        if (errors.Any())
        {
            await Publish(client, requestData, errors.First(), (int)errors.First().StatusCode, cancellationToken);
        } else
        {
            var responseBody = await handler.Handle(request);

            await Publish(client, requestData, responseBody, statusCode, cancellationToken);
        }
    }

    private static (TRequest?, IEnumerable<ValidationError>) ProcessRequest<TRequest>(Request<JsonNode> requestData, string[] pathParts, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger)
        where TRequest : new()
    {
        var errors = Enumerable.Empty<ValidationError>();
        TRequest mergedRequest;

        try
        {
            var requestBody = requestData.Body.Deserialize<TRequest>();
            (mergedRequest, var requestMergerErrors) = requestMerger(requestBody, requestData.QueryParameters, pathParts);

            errors = errors.Concat(requestMergerErrors);
        }
        catch (JsonException exception)
        {
            errors = errors.Append(new ValidationError
            {
                ErrorCode = ErrorCode.Format,
                StatusCode = HttpStatusCode.UnprocessableContent,
                Message = exception.Message,
                Location = exception.Path ?? string.Empty
            });

            mergedRequest = new TRequest();
        }

        return (mergedRequest, errors);
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
