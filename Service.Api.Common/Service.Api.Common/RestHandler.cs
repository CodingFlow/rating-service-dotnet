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
        async Task<Response<TResponse>> createResponse(TRequest request)
        {
            var responseBody = await postHandler.Handle(request);

            return new Response<TResponse>
            {
                StatusCode = 201,
                Body = responseBody,
                Headers = []
            };
        }

        await HandleRequest(client, requestData, pathParts, createResponse, requestMerger, cancellationToken);
    }

    public async Task HandlePost<TRequest>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest> postHandler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        async Task<Response<JsonObject>> createResponse(TRequest request)
        {
            await postHandler.Handle(request);

            return new Response<JsonObject>
            {
                StatusCode = 201,
                Body = [],
                Headers = []
            };
        }

        await HandleRequest(client, requestData, pathParts, createResponse, requestMerger, cancellationToken);
    }

    public async Task HandleGet<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> getHandler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        async Task<Response<TResponse>> createResponse(TRequest request)
        {
            var responseBody = await getHandler.Handle(request);

            return new Response<TResponse>
            {
                StatusCode = 200,
                Body = responseBody,
                Headers = []
            };
        }

        await HandleRequest(client, requestData, pathParts, createResponse, requestMerger, cancellationToken);
    }

    public async Task HandleDelete<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> deleteHandler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        async Task<Response<TResponse>> createResponse(TRequest request)
        {
            var responseBody = await deleteHandler.Handle(request);

            return new Response<TResponse>
            {
                StatusCode = 200,
                Body = responseBody,
                Headers = []
            };
        }

        await HandleRequest(client, requestData, pathParts, createResponse, requestMerger, cancellationToken);
    }

    public async Task HandleDelete<TRequest>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest> deleteHandler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        async Task<Response<JsonObject>> createResponse(TRequest request)
        {
            await deleteHandler.Handle(request);
            
            return new Response<JsonObject>
            {
                StatusCode = 204,
                Body = [],
                Headers = []
            };
        }

        await HandleRequest(client, requestData, pathParts, createResponse, requestMerger, cancellationToken);
    }

    private static async Task HandleRequest<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, Func<TRequest, Task<Response<TResponse>>> createResponse, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        var (request, errors) = ProcessRequest(requestData, pathParts, requestMerger);

        if (errors.Any())
        {
            var errorResponse = new Response<ValidationError>
            {
                StatusCode = (int)errors.First().StatusCode,
                Body = errors.First(),
                Headers = []
            };

            await Publish(client, requestData, errorResponse, cancellationToken);
        } else
        {
            await Publish(client, requestData, await createResponse(request), cancellationToken);
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

    private static async Task Publish<TResponse>(NatsClient client, Request<JsonNode> requestData, Response<TResponse> response, CancellationToken cancellationToken)
    {
        await client.PublishAsync(
            requestData.OriginReplyTo,
            response,
            cancellationToken: cancellationToken);
    }
}
