namespace Service.Application.Common.Handlers;

public interface IPostHandler<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request);
}
