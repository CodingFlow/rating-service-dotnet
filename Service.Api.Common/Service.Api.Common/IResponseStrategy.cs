using Service.Application.Common.Handlers;

namespace Service.Api.Common;

public interface IResponseStrategy<TRequest, TResponse>
{
    Task<Response<TResponse>> CreateResponse(TRequest request, IHandler<TRequest, TResponse> handler);
}
