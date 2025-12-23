namespace Service.Application.Common.Handlers;

public interface IGetHandler<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request);
}

