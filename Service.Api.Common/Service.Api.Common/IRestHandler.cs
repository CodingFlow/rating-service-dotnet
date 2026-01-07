using System.Text.Json.Nodes;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

public interface IRestHandler
{
    Task HandleDelete<TRequest, TResponse>(Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> handler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken) where TRequest : new();
    Task HandleGet<TRequest, TResponse>(Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> handler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken) where TRequest : new();
    Task HandlePost<TRequest, TResponse>(Request<JsonNode> requestData, string[] pathParts, IHandler<TRequest, TResponse> handler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken) where TRequest : new();
}