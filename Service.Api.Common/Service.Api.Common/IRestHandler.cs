using System.Text.Json.Nodes;
using NATS.Net;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

public interface IRestHandler
{
    Task HandlePost<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> postHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken);
    Task HandlePost<TRequest>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest> postHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken);
    Task HandleGet<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> getHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken);
    Task HandleDelete<TRequest, TResponse>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> getHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken);
    Task HandleDelete<TRequest>(NatsClient client, Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest> getHandler, Func<TRequest, Dictionary<string, string>, string[], TRequest> requestMerger, CancellationToken cancellationToken);
}