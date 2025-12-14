using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using NATS.Client.JetStream;
using NATS.Net;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

public interface IRestHandler
{
    Task HandleGet<TResponse>(NatsClient client, NatsJSMsg<Request<JsonNode>> message, IGetHandler<TResponse> getHandler, CancellationToken cancellationToken);
    Task HandlePost<TRequest, TResponse>(NatsClient client, NatsJSMsg<Request<JsonNode>> message, IPostHandler<TRequest, TResponse> postHandler, JsonTypeInfo<TRequest> requestJsonTypeInfo, CancellationToken cancellationToken);
}