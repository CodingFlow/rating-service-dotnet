using System.Text.Json.Nodes;
using NATS.Net;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

public interface IRestHandler
{
    Task HandlePost<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IPostHandler<TRequest, TResponse> postHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken);
    Task HandlePost<TRequest>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IPostHandler<TRequest> postHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken);
    Task HandleGet<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IGetHandler<TRequest, TResponse> getHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken);
    Task HandleGet<TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IGetHandler<TResponse> getHandler, CancellationToken cancellationToken);
}