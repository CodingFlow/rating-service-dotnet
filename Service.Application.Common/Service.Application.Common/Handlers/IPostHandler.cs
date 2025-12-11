namespace Service.Application.Common.Handlers;

public interface IPostHandler<TRequest, TResponse>
{
    TResponse Handle(TRequest request);
}
