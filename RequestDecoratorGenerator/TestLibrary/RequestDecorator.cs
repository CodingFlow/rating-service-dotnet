using RequestDecoratorGenerator;
using Service.Api.Common;
using Service.Api.Common.ResponseStrategies;
using Service.Application.Common.Handlers;

namespace TestProject;

[RequestDecorator]
internal class RequestDecorator<TRequest, TResponse>(IResponseStrategy<TRequest, TResponse> responseStrategy) : IResponseStrategy<TRequest, TResponse>
{
    public async Task<Result<Response<TResponse>>> CreateResponse(TRequest request, IHandler<TRequest, TResponse> handler)
    {
        return await responseStrategy.CreateResponse(request, handler);
    }
}
