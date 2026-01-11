using System.Net;
using System.Text.Json.Nodes;
using Service.Application.Common.Handlers;

namespace Service.Api.Common;

public class DeleteResponseStrategy<TRequest, TResponse> : IResponseStrategy<TRequest, TResponse>
{
    public async Task<Result<Response<TResponse>>> CreateResponse(TRequest request, IHandler<TRequest, TResponse> handler)
    {
        var responseBody = await handler.Handle(request);

        var statusCode = responseBody is JsonObject
            ? HttpStatusCode.NoContent
            : HttpStatusCode.OK;

        return new Response<TResponse>
        {
            StatusCode = statusCode,
            Body = responseBody,
            Headers = []
        };
    }
}
