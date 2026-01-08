using System.Text.Json.Nodes;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

public interface IRestHandler
{
    Task HandleRequest<TRequest, TResponse>(Request<JsonNode> requestData, string[] pathParts, IResponseStrategy<TRequest, TResponse> responseStrategy, IHandler<TRequest, TResponse> handler, Func<TRequest, Dictionary<string, string>, string[], (TRequest, IEnumerable<ValidationError>)> requestMerger, CancellationToken cancellationToken) where TRequest : new();
}