using Service.Application.Common.Handlers;

namespace Service.Api.Common;

public class GetResponseStrategy<TRequest, TResponse> : IResponseStrategy<TRequest, TResponse>
{
    public async Task<Result<Response<TResponse>>> CreateResponse(TRequest request, IHandler<TRequest, TResponse> handler)
    {
        var responseBody = await handler.Handle(request);

        return new Response<TResponse>
        {
            StatusCode = 200,
            Body = responseBody,
            Headers = []
        };
    }
}
