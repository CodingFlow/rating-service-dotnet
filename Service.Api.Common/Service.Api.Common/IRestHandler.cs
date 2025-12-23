using System.Text.Json.Nodes;
using NATS.Client.JetStream;
using NATS.Net;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

public interface IRestHandler
{
    Task HandlePost<TRequest, TResponse>(NatsClient client, INatsJSMsg<Request<JsonNode>> message, string[] pathParts, IPostHandler<TRequest, TResponse> postHandler, Func<TRequest, string[], TRequest> requestMerger, CancellationToken cancellationToken);
    Task HandlePost<TRequest>(NatsClient client, INatsJSMsg<Request<JsonNode>> message, string[] pathParts, IPostHandler<TRequest> postHandler, Func<TRequest, string[], TRequest> requestMerger, CancellationToken cancellationToken);
    Task HandleGet<TRequest, TResponse>(NatsClient client, INatsJSMsg<Request<JsonNode>> message, string[] pathParts, IGetHandler<TRequest, TResponse> getHandler, Func<TRequest, string[], TRequest> requestMerger, CancellationToken cancellationToken);
    Task HandleGet<TResponse>(NatsClient client, INatsJSMsg<Request<JsonNode>> message, string[] pathParts, IGetHandler<TResponse> getHandler, CancellationToken cancellationToken);
}