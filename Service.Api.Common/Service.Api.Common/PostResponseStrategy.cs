using Service.Application.Common.Handlers;

namespace Service.Api.Common;

public class PostResponseStrategy<TRequest, TResponse> : IResponseStrategy<TRequest, TResponse>
{
    public async Task<Response<TResponse>> CreateResponse(TRequest request, IHandler<TRequest, TResponse> handler)
    {
        var responseBody = await handler.Handle(request);

        return new Response<TResponse>
        {
            StatusCode = 201,
            Body = responseBody,
            Headers = []
        };
    }
}
