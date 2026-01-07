using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

internal class RestHandler(INatsConnectionService natsConnectionService) : IRestHandler
{
    public async Task HandlePost<TRequest, TResponse>(Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> handler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        var responseStrategy = new PostResponseStrategy<TRequest, TResponse>();

        await HandleRequest(requestData, pathParts, responseStrategy, handler, requestMerger, cancellationToken);
    }

    public async Task HandleGet<TRequest, TResponse>(Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> handler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        var responseStrategy = new GetResponseStrategy<TRequest, TResponse>();

        await HandleRequest(requestData, pathParts, responseStrategy, handler, requestMerger, cancellationToken);
    }

    public async Task HandleDelete<TRequest, TResponse>(Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> handler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken)
        where TRequest : new()
    {
        var responseStrategy = new DeleteResponseStrategy<TRequest, TResponse>();

        await HandleRequest(requestData, pathParts, responseStrategy, handler, requestMerger, cancellationToken);
    }

    private async Task HandleRequest<TRequest, TResponse>(
        Request<JsonNode> requestData,
        string[] pathParts,
        IResponseStrategy<TRequest, TResponse> responseStrategy,
        IHandler<TRequest, TResponse> handler,
        Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger,
        CancellationToken cancellationToken)
        where TRequest : new()
    {
        var (request, errors) = ParseRequestBody(requestData, pathParts, requestMerger);

        if (errors.Any())
        {
            var errorResponse = new Response<ValidationError>
            {
                StatusCode = (int)errors.First().StatusCode,
                Body = errors.First(),
                Headers = []
            };

            await Publish(requestData, errorResponse, cancellationToken);
        } else
        {
            var response = await responseStrategy.CreateResponse(request, handler);
            await Publish(requestData, response, cancellationToken);
        }
    }

    private static (TRequest?, IEnumerable<ValidationError>) ParseRequestBody<TRequest>(Request<JsonNode> requestData, string[] pathParts, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger)
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

    private async Task Publish<TResponse>(Request<JsonNode> requestData, Response<TResponse> response, CancellationToken cancellationToken)
    {
        await natsConnectionService.Client.PublishAsync(
            requestData.OriginReplyTo,
            response,
            cancellationToken: cancellationToken);
    }
}
