namespace Service.Application.Common.Handlers;

public interface IHandler<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request);
}
