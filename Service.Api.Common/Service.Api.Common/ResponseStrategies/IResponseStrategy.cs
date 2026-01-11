using Service.Application.Common.Handlers;

namespace Service.Api.Common.ResponseStrategies;

public interface IResponseStrategy<TRequest, TResponse>
{
    Task<Result<Response<TResponse>>> CreateResponse(TRequest request, IHandler<TRequest, TResponse> handler);
}
