using System.Text.Json.Nodes;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

public class DeleteResponseStrategy<TRequest, TResponse> : IResponseStrategy<TRequest, TResponse>
{
    public async Task<Response<TResponse>> CreateResponse(TRequest request, IHandler<TRequest, TResponse> handler)
    {
        var responseBody = await handler.Handle(request);

        var statusCode = responseBody is JsonObject
            ? 204
            : 200;

        return new Response<TResponse>
        {
            StatusCode = statusCode,
            Body = responseBody,
            Headers = []
        };
    }
}
